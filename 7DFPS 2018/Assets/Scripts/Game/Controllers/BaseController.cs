using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int taggedOre;
    public int retrievingOre;
    public int ore;

    public CollectingState collectingState = CollectingState.Idle;
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
        saveData.baseData.taggedOre = this.taggedOre;
        saveData.baseData.retrievingOre = this.retrievingOre;
        saveData.baseData.collectingState = (byte)collectingState;
    }

    private void OnGameLoaded(SaveData saveData)
    {
        transform.position = saveData.baseData.position;
        this.ore = saveData.baseData.ore;
        this.taggedOre = saveData.baseData.taggedOre;
        this.retrievingOre = saveData.baseData.retrievingOre;
        this.collectingState = (CollectingState)saveData.baseData.collectingState;
    }

    private void Update()
    {
        if(!GameManager.GamePaused)
            CollectOres();
    }

    private void CollectOres()
    {
        switch(collectingState)
        {
            case CollectingState.CollectingTaggedOres:
                {
                    if (taggedOre > 0)
                    {
                        taggedOre--;
                        retrievingOre++;
                    }
                    else
                        collectingState = CollectingState.AddingToInventory;
                }
                break;
            case CollectingState.AddingToInventory:
                {
                    if (retrievingOre > 0)
                    {
                        retrievingOre--;
                        ore++;
                    }
                    else
                        collectingState = CollectingState.Idle;
                }
                break;
        }
    }

    public enum CollectingState : byte
    {
        Idle,
        CollectingTaggedOres,
        AddingToInventory,
    }
}