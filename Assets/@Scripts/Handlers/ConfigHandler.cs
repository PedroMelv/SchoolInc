using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigHandler : WindowHandler
{
    public void DeleteSaveButton()
    {
        SaveLoadSystem.Instance.ResetGame();
        SaveLoadSystem.Instance.DeleteGame();
        SaveLoadSystem.Instance.NewGame("Retro");

        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
