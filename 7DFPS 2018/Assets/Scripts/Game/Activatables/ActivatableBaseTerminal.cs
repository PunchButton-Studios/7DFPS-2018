using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableBaseTerminal : Activatable
{
    public GameObject terminalCanvas;
    public BaseSlot[] groundSlots, wallSlots;

    public BaseObject[] baseObjects;

    public override string ActivateInfo
    {
        get
        {
            return "Edit Base";
        }
    }

    private void Awake()
    {
        GameManager.Main.loadGameEvent += OnGameLoaded;
        GameManager.Main.saveGameEvent += OnSaveGame;
    }

    private void OnSaveGame(SaveData saveData)
    {
        saveData.baseData.groundObjects = new SaveData.BaseData.ObjectData[groundSlots.Length];
        for (int i = 0; i < groundSlots.Length; i++)
        {
            saveData.baseData.groundObjects[i] = new SaveData.BaseData.ObjectData()
            {
                id = Array.IndexOf(baseObjects, groundSlots[i].BaseObject),
                extraData = groundSlots[i].extraData
            };
        }

        saveData.baseData.wallObjects = new SaveData.BaseData.ObjectData[wallSlots.Length];
        for (int i = 0; i < wallSlots.Length; i++)
        {
            saveData.baseData.wallObjects[i] = new SaveData.BaseData.ObjectData()
            {
                id = Array.IndexOf(baseObjects, wallSlots[i].BaseObject),
                extraData = wallSlots[i].extraData
            };
        }
    }

    private void OnGameLoaded(SaveData saveData)
    {
        for (int i = 0; i < groundSlots.Length && i < saveData.baseData.groundObjects.Length; i++)
        {
            if (saveData.baseData.groundObjects[i].id != -1)
            {
                groundSlots[i].BaseObject = baseObjects[saveData.baseData.groundObjects[i].id];
                groundSlots[i].extraData = saveData.baseData.groundObjects[i].extraData;
                groundSlots[i].ApplyExtraData();
            }
        }
        for (int i = 0; i < wallSlots.Length && i < saveData.baseData.wallObjects.Length; i++)
        {
            if (saveData.baseData.wallObjects[i].id != -1)
            {
                wallSlots[i].BaseObject = baseObjects[saveData.baseData.wallObjects[i].id];
                wallSlots[i].extraData = saveData.baseData.wallObjects[i].extraData;
                wallSlots[i].ApplyExtraData();
            }
        }
    }

    public override void Activate(EntityPlayer player)
    {
        terminalCanvas.SetActive(true);
        GameManager.Main.OpenMenu(terminalCanvas.GetComponent<ICloseableMenu>());
    }
}