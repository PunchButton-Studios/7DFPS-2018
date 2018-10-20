using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvas;

    private void Reset()
    {
        canvas = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        GameManager.pauseEvent += OnPauseChange;
        canvas.SetActive(GameManager.GamePaused);
    }

    private void OnPauseChange(bool isPaused) => canvas.SetActive(isPaused);

    public void ResumeButton() => GameManager.GamePaused = false;

    public void ExitGameButton() => Application.Quit();
}