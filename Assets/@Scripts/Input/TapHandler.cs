using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapHandler : MonoBehaviour
{
    public GameObject currentObject;

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
        HandleTap(touch.position);
    }

    private void HandleTap(Vector3 position)
    {
        Ray camRay = Camera.main.ScreenPointToRay(position);

        if (Physics.Raycast(camRay, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                TappableObject tap = hit.collider.GetComponent<TappableObject>();
                if (tap == null) tap = hit.collider.GetComponentInParent<TappableObject>();
                if (tap == null) tap = hit.collider.GetComponentInChildren<TappableObject>();

                tap?.Tap();

                currentObject = hit.collider.gameObject;
            }
        }
    }
}
