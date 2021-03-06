﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableOre : Activatable, IWorldGenObject, ISonarResponder
{
    public string activateText = "Tag Cobalt Ore";
    public GameObject tagObjectPrefab;
    public AudioSource tagSfx, sonarSfx;
    private bool tagged;

    public int ore = 5;
    public float koboldSpawnDecrease = 1.0f;

    public override string ActivateInfo
    {
        get
        {
            return activateText;
        }
    }

    public AudioSource SonarResponseSource
    {
        get
        {
            return tagged ? null : sonarSfx;
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
        BaseController.Main.taggedOre += ore;
        tagSfx.Play();
        KoboldSpawner.Main.timer -= koboldSpawnDecrease;
    }

    private void Tag()
    {
        if (!tagged)
        {
            tagged = true;
            GameObject tagObject = Instantiate(tagObjectPrefab, transform.position, transform.rotation);
            tagObject.transform.parent = transform;
        }
    }

    public override bool CanBeActivated(EntityPlayer player) => !tagged;
}