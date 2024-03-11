using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraObject;

    private void Start()
    {
        cameraObject = Camera.main.gameObject.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = cameraObject.rotation;
    }
}
