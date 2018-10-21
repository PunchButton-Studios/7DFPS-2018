using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvas;
    public string mainMenuScene;

    private void Reset()
    {
        canvas = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        GameManager.Main.pauseEvent += OnPauseChange;
        canvas.SetActive(GameManager.GamePaused);
    }

    private void OnPauseChange(bool isPaused)
    {
        if(GameManager.Main.ActiveMenu == null)
            canvas.SetActive(isPaused);
    }

    public void ResumeButton() => GameManager.GamePaused = false;

    public void ExitGameButton()
    {
        GameManager.Main.SaveGame();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
}