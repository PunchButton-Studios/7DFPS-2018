using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int ore;

    public static BaseController Main { get; private set; }

    private void Awake()
    {
        Main = this;
        GameManager.Main.loadGameEvent += OnGameLoaded;
        GameManager.Main.saveGameEvent += OnSaveGame;
    }

    private void OnSaveGame(SaveData saveData)
    {
        saveData.baseData.position = transform.position;
        saveData.baseData.ore = this.ore;
    }

    private void OnGameLoaded(SaveData saveData)
    {
        transform.position = saveData.baseData.position;
        this.ore = saveData.baseData.ore;
    }
}