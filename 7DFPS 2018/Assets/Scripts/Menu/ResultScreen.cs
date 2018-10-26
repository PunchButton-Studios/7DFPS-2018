using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    public string menuSceneName;
    public Text depthText;
    public static uint depth;

    private void Awake()
    {
        depthText.text = $"{(depth + 1) * 100} m";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }
}