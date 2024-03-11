using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField] private float cameraLerpSpeed;
    [SerializeField] private float moveStrength;
    [SerializeField] private Vector3 maxPosition;
    [SerializeField] private Vector3 minPosition;
    [Space]
    [SerializeField] private float zoomStrength;
    [SerializeField] private float zoomMin;
    [SerializeField] private float zoomMax;
    
    [SerializeField] private float zoomMinRotation;
    [SerializeField] private float zoomMaxRotation;


    private Vector3 cameraPosition;
    private float cameraZoom;
    private float cameraRotation;

    private Vector3 moveStartPos;
    private Vector3 moveCurrentPos;

    private Vector3 pinchPosA;
    private Vector3 pinchPosB;
    private float lastDistancePinch;

    private void Start()
    {
        InitializeInput();

        cameraPosition = transform.position;
        cameraZoom = transform.position.y;
        cameraRotation = transform.eulerAngles.x;

    }

    private void LateUpdate()
    {
        Vector3 pos = cameraPosition;
        pos.y = cameraZoom;
        cameraPosition = pos;

        if (Vector3.Distance(transform.position, cameraPosition) > .1f)
            transform.position = Vector3.Lerp(transform.position, cameraPosition, cameraLerpSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraRotation, -45f, 0f), cameraLerpSpeed * Time.deltaTime);
    }

    [ContextMenu("Validate Position")]
    private void ValidatePosition()
    {
        cameraZoom = transform.position.y;
        cameraRotation = Mathf.Clamp(zoomMaxRotation * (cameraZoom / zoomMax), zoomMinRotation, zoomMaxRotation);

        Vector3 pos = transform.position;
        pos.y = cameraZoom;
        cameraPosition = pos;


        transform.position = cameraPosition;
        transform.rotation = Quaternion.Euler(cameraRotation, -45f, 0f);
    }

    private void InitializeInput()
    {
        InputHandler input = InputHandler.Instance;

        input.onSingleTouchStart += SetupMoveCamera;
        input.onSingleTouchMove += MoveCamera;

        input.onTwoTouchesStart += StartZoom;
        input.onTwoTouchesChanged += UpdateZoom;

        #if UNITY_EDITOR

        input.onBeginDrag += SetupDragCamera;
        input.onMoveDrag += UpdateDragCamera;

        input.onMouseWheel += UpdateZoomWheel;

        #endif

    }

    private void SetupMoveCamera(Touch touch)
    {
        moveStartPos = touch.position;
    }
    private void MoveCamera(Touch touch)
    {
        moveCurrentPos = touch.position;
        if (Vector3.Distance(moveStartPos, moveCurrentPos) < 10f) return;

        Vector3 moveDiff = moveStartPos - moveCurrentPos;

        moveDiff = transform.TransformDirection(moveDiff);

        moveDiff.y = 0;
        //moveDiff *= .75f;

        cameraPosition += (moveDiff * moveStrength) * (cameraZoom / zoomMax);

        float clampedX = Mathf.Clamp(cameraPosition.x, minPosition.x, maxPosition.x);
        float clampedZ = Mathf.Clamp(cameraPosition.z, minPosition.z, maxPosition.z);

        cameraPosition = new Vector3(clampedX, cameraZoom, clampedZ);

        moveStartPos = touch.position;
    }

    private void StartZoom(Touch t1, Touch t2)
    {
        pinchPosA = t1.position;
        pinchPosB = t2.position;
        lastDistancePinch = Vector3.Distance(pinchPosA, pinchPosB);
    }
    private void UpdateZoom(Touch t1, Touch t2)
    {
        if (Vector3.Distance(pinchPosA, t1.position) > 10f)
        {
            pinchPosA = t1.position;
        }
        if (Vector3.Distance(pinchPosB, t2.position) > 10f)
        {
            pinchPosB = t2.position;
        }

        float distance = Vector3.Distance(pinchPosA, pinchPosB);

        cameraZoom -= (distance - lastDistancePinch) * zoomStrength;

        cameraZoom = Mathf.Clamp(cameraZoom, zoomMin, zoomMax);
        cameraRotation = Mathf.Clamp(zoomMaxRotation * (cameraZoom / zoomMax), zoomMinRotation, zoomMaxRotation);

        lastDistancePinch = distance;
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

        cameraPosition += (moveDiff * moveStrength) * (cameraZoom / zoomMax);

        float clampedX = Mathf.Clamp(cameraPosition.x, minPosition.x, maxPosition.x);
        float clampedZ = Mathf.Clamp(cameraPosition.z, minPosition.z, maxPosition.z);

        cameraPosition = new Vector3(clampedX, cameraZoom, clampedZ);

        moveStartPos = mousePos;
    }

    private void UpdateZoomWheel(float mouseWheel)
    {
        cameraZoom -= mouseWheel * 5f;

        cameraZoom = Mathf.Clamp(cameraZoom, zoomMin, zoomMax);
        cameraRotation = Mathf.Clamp(zoomMaxRotation * (cameraZoom / zoomMax), zoomMinRotation, zoomMaxRotation);

    }

#endif

    private void OnDrawGizmos()
    {
        Vector3 boxCenter = (minPosition + maxPosition) / 2f;
        boxCenter.y = (zoomMin + zoomMax) / 2f;

        Vector3 boxSize = maxPosition - minPosition;
        boxSize.y = zoomMax - zoomMin;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
