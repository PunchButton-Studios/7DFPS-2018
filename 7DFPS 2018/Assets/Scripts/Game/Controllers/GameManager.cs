using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string PAUSE_BUTTON_NAME = "Pause";

    private bool isPaused = false;
    private float oldTimescale = 1.0f;

    private static GameManager main;

    public static event OnPause pauseEvent;

    public static GameManager Main
    {
        get
        {
            return main ?? (main = new GameObject("Game Manager").AddComponent<GameManager>());
        }
    }

    public static bool GamePaused
    {
        get
        {
            return Main.isPaused;
        }
        set
        {
            Main.Pause(value);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.AddComponent<DebugHandler>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(PAUSE_BUTTON_NAME))
            Pause(!isPaused);
    }

    public void Pause(bool pauseState)
    {
        if (pauseState == isPaused)
            return; //Ignore

        if (pauseState)
            oldTimescale = Time.timeScale;
        Time.timeScale = pauseState ? 0.0f : oldTimescale;
        Cursor.lockState = pauseState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pauseState;

        isPaused = pauseState;
        pauseEvent?.Invoke(pauseState);
    }
}