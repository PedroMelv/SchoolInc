using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : Singleton<InputHandler>
{
    private float tapBuffer = .33f;
    private bool doubleTouch = false;

    private bool onSingleStart = false;

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

    #if UNITY_EDITOR
    public float clickBuffer = .33f;    
    #endif

    private void Update()
    {
        #if UNITY_EDITOR

        EditorInput();

        #endif

        if (Input.touchCount > 0) tapBuffer -= Time.deltaTime;
        else
        {
            tapBuffer = .33f;
            doubleTouch = false;
        }

        switch (Input.touchCount)
        {
            case 1:
                if(!doubleTouch) SingleTouch(); else onSingleTouchStart?.Invoke(Input.GetTouch(0));
                doubleTouch = false;
            break;

            case 2:
                DoubleTouch();
            break;
        }
    }

    private void SingleTouch()
    {
        switch (Input.GetTouch(0).phase)
        {
            case TouchPhase.Began:
                onSingleTouchStart?.Invoke(Input.GetTouch(0));
                break;
            case TouchPhase.Moved:
                if (EventSystem.current.IsPointerOverGameObject(0)) return;
                onSingleTouchMove?.Invoke(Input.GetTouch(0));
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Ended:
                onSingleTouchEnded?.Invoke(Input.GetTouch(0));

                if (tapBuffer > 0f)
                {
                    onTap?.Invoke(Input.GetTouch(0));
                }

                break;
            case TouchPhase.Canceled:
                onSingleTouchEnded?.Invoke(Input.GetTouch(0));
                break;
        }
    }

    private void DoubleTouch()
    {
        if (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1)) return;

        if (!doubleTouch)
        {
            onTwoTouchesStart?.Invoke(Input.GetTouch(0), Input.GetTouch(1));
            doubleTouch = true;
        }
        else
        {
            onTwoTouchesChanged?.Invoke(Input.GetTouch(0), Input.GetTouch(1));
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