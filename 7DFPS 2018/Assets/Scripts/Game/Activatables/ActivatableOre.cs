using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableOre : Activatable, IWorldGenObject
{
    public string activateText = "Tag Ore";
    public GameObject tagObjectPrefab;
    private bool tagged;

    public override string ActivateInfo
    {
        get
        {
            return activateText;
        }
    }

    public int Id { get; set; }

    private void Awake()
    {
        GameManager.Main.saveGameEvent += OnSaveGame;
    }

    private void OnSaveGame(SaveData saveData)
    {
        if(tagged)
            saveData.worldData.SaveExtraData(this, 1);
    }

    public void OnWorldLoaded(byte saveData)
    {
        if (saveData > 0)
            Tag();
    }

    public override void Activate(EntityPlayer player)
    {
        Tag();
    }

    private void Tag()
    {
        if (!tagged)
        {
            tagged = true;
            Instantiate(tagObjectPrefab, transform.position, transform.rotation);
        }
    }

    public override bool CanBeActivated(EntityPlayer player) => !tagged;
}