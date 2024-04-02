using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private const string k_speedTapTag = "SpeedTap";

    private Dictionary<int, TouchInfo> touchesOnScreen = new Dictionary<int, TouchInfo>();

    private class TouchInfo
    {
        public int touchIndex;
        public float touchTime;
        public float touchSpeedTime;
        public bool touchIsOnUI;
        public bool speedTapping;

        public bool dragging;

        public TouchInfo(int touchIndex, float touchTime, bool touchIsOnUI)
        {
            touchSpeedTime = 0f;
            this.touchIndex = touchIndex;
            this.touchTime = touchTime;
            this.touchIsOnUI = touchIsOnUI;
        }

        public void StartSpeedTapping()
        {
            if(!speedTapping)Instance.StartCoroutine(ESpeedTapping());
        }

        public void StopSpeedTapping()
        {
            touchSpeedTime = 0f;
            speedTapping = false;
        }

        private IEnumerator ESpeedTapping()
        {
            if(!EventSystem.current.IsPointerOverGameObject(touchIndex)) yield break;

            GameObject currentObject = EventSystem.current.currentSelectedGameObject;
            if (currentObject == null || !currentObject.CompareTag(k_speedTapTag)) yield break;

            Button button = currentObject.GetComponent<Button>();
            if (button == null) yield break;

            speedTapping = true;
            float tapTimming = .25f;
            while(speedTapping)
            {
                if(button.interactable)button.onClick?.Invoke();
                yield return new WaitForSeconds(tapTimming);
                tapTimming -= .05f;
                tapTimming = Mathf.Clamp(tapTimming, 0.05f, 1f);
            }
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

                if (!touchesOnScreen.ContainsKey(fingerID)) touchesOnScreen.Add(fingerID, new TouchInfo(fingerID, 0f, touchUI));

                if (!touchesOnScreen[fingerID].touchIsOnUI)
                {
                    onSingleTouchStart?.Invoke(touch);
                }

                break;
            case TouchPhase.Moved:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].touchTime += Time.deltaTime;
                    
                    touchesOnScreen[fingerID].touchIsOnUI = touchUI;


                    if (!touchesOnScreen[fingerID].touchIsOnUI || touchesOnScreen[fingerID].dragging)
                    {
                        touchesOnScreen[fingerID].dragging = true;
                        onSingleTouchMove?.Invoke(touch);
                    }
                }

                break;
            case TouchPhase.Stationary:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].touchTime += Time.deltaTime;

                    touchesOnScreen[fingerID].touchIsOnUI = touchUI;

                    if (touchesOnScreen[fingerID].dragging) return;

                    if (touchesOnScreen[fingerID].touchIsOnUI && touchesOnScreen[fingerID].touchSpeedTime >= .5f)
                    {
                        touchesOnScreen[fingerID].StartSpeedTapping();
                    }else if(touchesOnScreen[fingerID].touchIsOnUI)
                    {
                        touchesOnScreen[fingerID].touchSpeedTime += Time.deltaTime;
                    }
                    else
                    {
                        touchesOnScreen[fingerID].touchSpeedTime = 0f;
                    }
                }
                break;
            case TouchPhase.Ended:

                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].StopSpeedTapping();

                    onSingleTouchEnded?.Invoke(touch);
                    if (touchesOnScreen[fingerID].touchTime <= .33f && !touchesOnScreen[fingerID].touchIsOnUI) onTap?.Invoke(touch);
                    
                    touchesOnScreen.Remove(fingerID);
                }
                break;
            case TouchPhase.Canceled:
                if (touchesOnScreen.ContainsKey(fingerID))
                {
                    touchesOnScreen[fingerID].StopSpeedTapping();

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
