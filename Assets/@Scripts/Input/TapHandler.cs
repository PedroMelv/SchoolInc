using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapHandler : MonoBehaviour
{
    private void Start()
    {
        InitializeInput();
    }

    private void InitializeInput()
    {
        InputHandler.Instance.onTap += HandleTap;

#if UNITY_EDITOR

        InputHandler.Instance.onReleaseClick += HandleTap;

#endif
    }

    private void HandleTap(Touch touch)
    {
        Ray camRay = Camera.main.ScreenPointToRay(touch.position);

        if(Physics.Raycast(camRay, out RaycastHit hit))
        {
            if(hit.collider != null)
            {
                TappableObject tap = hit.collider.GetComponent<TappableObject>();
                if(tap == null) tap = hit.collider.GetComponentInParent<TappableObject>();
                if(tap == null) tap = hit.collider.GetComponentInChildren<TappableObject>();

                tap?.Tap();
            }
        }
    }

    private void HandleTap(Vector3 mousePos)
    {
        Ray camRay = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(camRay, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                TappableObject tap = hit.collider.GetComponent<TappableObject>();
                if (tap == null) tap = hit.collider.GetComponentInParent<TappableObject>();
                if (tap == null) tap = hit.collider.GetComponentInChildren<TappableObject>();

                tap?.Tap();
            }
        }
    }
}
