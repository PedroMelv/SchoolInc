using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField] private float cameraLerpSpeed;
    [SerializeField] private float moveStrength;
    [SerializeField] private Vector3 maxPosition;
    [SerializeField] private Vector3 minPosition;

    private Vector3 cameraPosition;

    private Vector3 moveStartPos;
    private Vector3 moveCurrentPos;

    private int touchHandling;

    private void Start()
    {
        InitializeInput();

        cameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 pos = cameraPosition;
        pos.y = transform.position.y;
        cameraPosition = pos;

        if (Vector3.Distance(transform.position, cameraPosition) > .1f)
            transform.position = Vector3.Lerp(transform.position, cameraPosition, cameraLerpSpeed * Time.deltaTime);
    }

    [ContextMenu("Validate Position")]
    private void ValidatePosition()
    {
        Vector3 pos = transform.position;
        pos.y = transform.position.y;
        cameraPosition = pos;

        transform.position = cameraPosition;
    }

    private void InitializeInput()
    {
        InputHandler input = InputHandler.Instance;

        input.onSingleTouchStart += SetupMoveCamera;
        input.onSingleTouchMove += MoveCamera;
        input.onSingleTouchEnded += ReleaseCamera;

        #if UNITY_EDITOR

        input.onBeginDrag += SetupDragCamera;
        input.onMoveDrag += UpdateDragCamera;

        #endif

    }

    private void SetupMoveCamera(Touch touch)
    {
        if (touchHandling != -1) return;
        moveStartPos = touch.position;
        touchHandling = touch.fingerId;
    }
    private void MoveCamera(Touch touch)
    {
        if (touchHandling != touch.fingerId) return;
        moveCurrentPos = touch.position;
        if (Vector3.Distance(moveStartPos, moveCurrentPos) < 10f) return;

        Vector3 moveDiff = moveStartPos - moveCurrentPos;

        moveDiff = moveDiff.x * transform.right + moveDiff.y * transform.forward;

        Debug.Log(moveDiff);
        //moveDiff *= .75f;

        cameraPosition += (moveDiff * moveStrength);

        float clampedX = Mathf.Clamp(cameraPosition.x, minPosition.x, maxPosition.x);
        float clampedZ = Mathf.Clamp(cameraPosition.z, minPosition.z, maxPosition.z);

        cameraPosition = new Vector3(clampedX, transform.position.y, clampedZ);

        moveStartPos = touch.position;
    }

    private void ReleaseCamera(Touch touch)
    {
        if (touchHandling != touch.fingerId) return;
        touchHandling = -1;
    }

    #if UNITY_EDITOR

    private void SetupDragCamera(Vector3 mousePos)
    {
        moveStartPos = mousePos;
    }
    private void UpdateDragCamera(Vector3 mousePos)
    {
        moveCurrentPos = mousePos;

        if (Vector3.Distance(moveStartPos, moveCurrentPos) < 10f) return;

        Vector3 moveDiff = moveStartPos - moveCurrentPos;

        moveDiff = transform.TransformDirection(moveDiff);

        moveDiff.y = 0;
        //moveDiff *= .75f;

        cameraPosition += moveDiff * moveStrength;

        float clampedX = Mathf.Clamp(cameraPosition.x, minPosition.x, maxPosition.x);
        float clampedZ = Mathf.Clamp(cameraPosition.z, minPosition.z, maxPosition.z);

        cameraPosition = new Vector3(clampedX, transform.position.y, clampedZ);

        moveStartPos = mousePos;
    }


#endif

    private void OnDrawGizmos()
    {
        Vector3 boxCenter = (minPosition + maxPosition) / 2f;
        boxCenter.y = transform.position.y / 2f;

        Vector3 boxSize = maxPosition - minPosition;
        boxSize.y = transform.position.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
