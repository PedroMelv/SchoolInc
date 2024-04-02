using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField] private InputHandler input;
    [SerializeField] private SchoolsManager schoolsManager;
    [SerializeField] private float cameraLerpSpeed;
    [SerializeField] private float moveStrength;
    [SerializeField] private Vector3 maxPosition;
    [SerializeField] private Vector3 minPosition;

    private Vector3 cameraPosition;

    private Vector3 moveStartPos;
    private Vector3 moveCurrentPos;

    private int touchHandling = -1;

    private CameraState currentState;
    private CameraState lastState;

    private void Start()
    {
        cameraPosition = transform.position;

        ChangeState(new CameraState_InputHandled());
    }

    private void LateUpdate()
    {
        currentState?.OnLateUpdate();
    }

    private void OnDisable()
    {
        if(currentState != null)
            currentState.OnExit();
    }

    public void ChangeState(CameraState targetState)
    {
        if (currentState == targetState) return;

        currentState?.OnExit();

        lastState = currentState;
        currentState = targetState;

        currentState?.OnEnter(this);
    }

    [ContextMenu("Validate Position")]
    private void ValidatePosition()
    {
        Vector3 pos = transform.position;
        pos.y = transform.position.y;
        cameraPosition = pos;

        transform.position = cameraPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 boxCenter = (minPosition + maxPosition) / 2f;
        boxCenter.y = transform.position.y / 2f;

        Vector3 boxSize = maxPosition - minPosition;
        boxSize.y = transform.position.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }


    public abstract class CameraState
    {
        protected CameraInput camera;
        public virtual void OnEnter(CameraInput camera)
        {
            this.camera = camera;
        }
        public virtual void OnExit()
        {

        }

        public virtual void OnUpdate()
        {

        }
        public virtual void OnLateUpdate()
        {

        }
    }

    public class CameraState_InputHandled : CameraState
    {

        public override void OnEnter(CameraInput camera)
        {
            base.OnEnter(camera);

            camera.touchHandling = -1;
            
            InitializeInput();
        }

        public override void OnExit()
        {
            base.OnExit();
            DeinitializeInput();
        }
        public override void OnLateUpdate()
        {
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            if (Vector3.Distance(camera.transform.position, camera.cameraPosition) > .1f)
                camera.transform.position = Vector3.Lerp(camera.transform.position, camera.cameraPosition, camera.cameraLerpSpeed * Time.deltaTime);
        }

        private void InitializeInput()
        {
            camera.input.onSingleTouchStart += SetupMoveCamera;
            camera.input.onSingleTouchMove += MoveCamera;
            camera.input.onSingleTouchEnded += ReleaseCamera;
        }

        private void DeinitializeInput()
        {
            camera.input.onSingleTouchStart -= SetupMoveCamera;
            camera.input.onSingleTouchMove -= MoveCamera;
            camera.input.onSingleTouchEnded -= ReleaseCamera;
        }


        private void SetupMoveCamera(Touch touch)
        {
            if (camera.touchHandling != -1) return;
            camera.moveStartPos = touch.position;
            camera.moveCurrentPos = touch.position;
            camera.touchHandling = touch.fingerId;
        }
        private void MoveCamera(Touch touch)
        {
            if (camera.touchHandling != touch.fingerId) return;
            camera.moveCurrentPos = touch.position;
            if (Vector3.Distance(camera.moveStartPos, camera.moveCurrentPos) < 10f) return;

            Vector3 moveDiff = camera.moveStartPos - camera.moveCurrentPos;

            moveDiff = moveDiff.x * camera.transform.right + moveDiff.y * camera.transform.forward;
            //moveDiff *= .75f;

            camera.cameraPosition += (moveDiff * camera.moveStrength);

            float clampedX = Mathf.Clamp(camera.cameraPosition.x, camera.minPosition.x, camera.maxPosition.x);
            float clampedZ = Mathf.Clamp(camera.cameraPosition.z, camera.minPosition.z, camera.maxPosition.z);

            camera.cameraPosition = new Vector3(clampedX, camera.transform.position.y, clampedZ);

            camera.moveStartPos = touch.position;
        }

        private void ReleaseCamera(Touch touch)
        {
            if (camera.touchHandling != touch.fingerId) return;
            camera.touchHandling = -1;
        }
    }

    public class CameraState_StoreFocused : CameraState
    {
        private Vector3 savedRotation;

        public override void OnEnter(CameraInput camera)
        {
            base.OnEnter(camera);

            savedRotation = camera.transform.eulerAngles;
        }
        public override void OnLateUpdate()
        {
            UpdateFocusStore();
        }

        private void UpdateFocusStore()
        {
            if (camera.schoolsManager.SchoolSelected == null) return;
            Vector3 targetPosition = camera.schoolsManager.SchoolSelected.Visual.CameraOffsetPosition;
            targetPosition += camera.schoolsManager.SchoolSelected.transform.position;

            if (Vector3.Distance(camera.transform.position, targetPosition) > .1f)
                camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, 10f * Time.deltaTime);
        }
    }
}
