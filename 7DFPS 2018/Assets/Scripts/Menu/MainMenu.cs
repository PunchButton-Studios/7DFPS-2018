using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject[] panels;
    public string gameScene;

    public void SwitchActivePanel(GameObject panel)
    {
        foreach (GameObject p in panels)
            p.SetActive(false);
        panel.SetActive(true);
    }

    public void ExitGame() => Application.Quit();

    public void NewGame(string saveName)
    {
        GameManager.saveName = saveName;
        GameManager.startState = GameManager.StartState.NewGame;
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    public void LoadGame(string saveName)
    {
        GameManager.saveName = saveName;
        GameManager.startState = GameManager.StartState.LoadGame;
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }
}