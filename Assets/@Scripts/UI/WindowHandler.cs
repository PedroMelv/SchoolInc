using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowHandler : MonoBehaviour
{
    [SerializeField] private GameObject _window;

    public void SwapWindow()
    {
        _window.SetActive(!_window.activeSelf);
    }

}
