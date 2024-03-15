using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : Singleton<InputHandler>
{

    public Action<Touch> onTap;
    public Action<Vector3> onReleaseClick;

    public Action<Touch> onSingleTouchStart;
    public Action<Touch> onSingleTouchMove;
    public Action<Touch> onSingleTouchEnded;

    public Action<Vector3> onBeginDrag;
    public Action<Vector3> onMoveDrag;
    public Action<Vector3> onEndDrag;

    public Action<float> onMouseWheel;

    public Action<Touch, Touch> onTwoTouchesStart;
    public Action<Touch, Touch> onTwoTouchesChanged;

    private Dictionary<int, TouchInfo> touchesOnScreen = new Dictionary<int, TouchInfo>();

    private class TouchInfo
    {
        public float touchTime;
        public bool touchIsOnUI;
    
        public TouchInfo(float touchTime, bool touchIsOnUI)
        {
            this.touchTime = touchTime;
            this.touchIsOnUI = touchIsOnUI;
        }
    }

    #if UNITY_EDITOR
    public float clickBuffer = .33f;
#endif

    public CameraInput cameraInput;

    private void Start()
    {
        if (cameraInput == null) cameraInput = FindObjectOfType<CameraInput>();
    }

    private void Update()
    {
        foreach (var touch in Input.touches)
        {
            SingleTouch(touch);
        }
    }

    private void SingleTouch(Touch touch)
    {
        int fingerID = touch.fingerId;

        bool touchUI = EventSystem.current.IsPointerOverGameObject(fingerID);

        switch (touch.phase)
        {
            case TouchPhase.Began:

                if (!touchesOnScreen.ContainsKey(fingerID)) touchesOnScreen.Add(fingerID, new TouchInfo(0f, touchUI));

                if (!touchesOnScreen[fingerID].touchIsOnUI) 
                    onSingleTouchStart?.Invoke(touch);

                break;
            case TouchPhase.Moved:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].touchTime += Time.deltaTime;

                    touchesOnScreen[fingerID].touchIsOnUI = touchUI;


                    if (!touchesOnScreen[fingerID].touchIsOnUI)
                        onSingleTouchMove?.Invoke(touch);
                }

                

                break;
            case TouchPhase.Stationary:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].touchTime += Time.deltaTime;

                    touchesOnScreen[fingerID].touchIsOnUI = touchUI;
                }

                break;
            case TouchPhase.Ended:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    if (!touchesOnScreen[fingerID].touchIsOnUI)
                    {
                        onSingleTouchEnded?.Invoke(touch);
                        if (touchesOnScreen[fingerID].touchTime <= .33f) onTap?.Invoke(touch);
                    }
                    touchesOnScreen.Remove(fingerID);
                }
                break;
            case TouchPhase.Canceled:
                if (touchesOnScreen.ContainsKey(fingerID))
                { 
                    if (!touchesOnScreen[fingerID].touchIsOnUI)
                        onSingleTouchEnded?.Invoke(touch);

                    touchesOnScreen.Remove(fingerID);
                }

                break;
        }
    }

    #if UNITY_EDITOR

    private void EditorInput()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            clickBuffer = .33f;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            onBeginDrag?.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && clickBuffer >= 0f)
        {
            onReleaseClick?.Invoke(Input.mousePosition);
            onEndDrag?.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            clickBuffer -= Time.deltaTime;
            onMoveDrag?.Invoke(Input.mousePosition);
        }
        else
        {
            clickBuffer = .33f;
        }

        onMouseWheel?.Invoke(Input.mouseScrollDelta.y);
    }

    #endif  
}
