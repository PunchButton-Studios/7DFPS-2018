using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour, ICloseableMenu
{
    public GameObject canvas;
    public string mainMenuScene;

    private void Reset()
    {
        canvas = transform.GetChild(0).gameObject;
    }

    public void ResumeButton() => Close();

    public void ExitGameButton()
    {
        GameManager.Main.SaveGame();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }

    public void Close()
    {
        canvas.SetActive(false);
        GameManager.Main.CloseMenu();
    }
}