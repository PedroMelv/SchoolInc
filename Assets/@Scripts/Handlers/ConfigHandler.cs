using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigHandler : MonoBehaviour
{
    [SerializeField] private GameObject _configWindow;

    public void SwapConfigWindow()
    {
        _configWindow.SetActive(!_configWindow.activeSelf);
    }

    public void DeleteSaveButton()
    {
        SaveLoadSystem.Instance.ResetGame();
        SaveLoadSystem.Instance.DeleteGame();

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
