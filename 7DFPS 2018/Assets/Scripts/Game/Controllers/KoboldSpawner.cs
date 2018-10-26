using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoboldSpawner : MonoBehaviour
{
    private List<GameObject> koboldList = new List<GameObject>();
    public GameObject koboldPrefab;
    public int maxKobolds = 10;
    public float minInterval = 60.0f, maxInterval = 260.0f;
    public float timer = 0.0f;
    public float minRange = 10.0f, maxRange = 30.0f;

    public static KoboldSpawner Main
    {
        get;
        private set;
    }

    public int KoboldCount
    {
        get
        {
            koboldList.RemoveAll((k) => k == null);
            return koboldList.Count;
        }
    }

    private void Awake()
    {
        Main = this;
        timer = Random.Range(minInterval, maxInterval);
        GameManager.Main.saveGameEvent += OnSaveGame;
        GameManager.Main.loadGameEvent += OnLoadGame;
    }

    private void OnLoadGame(SaveData saveData)
    {
        for (int i = 0; i < saveData.worldData.kobolds; i++)
            SpawnKobold();
        this.timer = saveData.worldData.koboldTimer;
    }

    private void OnSaveGame(SaveData saveData)
    {
        koboldList.RemoveAll((k) => k == null);
        saveData.worldData.kobolds = koboldList.Count;
        saveData.worldData.koboldTimer = timer;
    }

    private void Update()
    {
        if (GameManager.GamePaused)
            return;
        timer -= Time.deltaTime;

        if(timer < 0.0f)
        {
            koboldList.RemoveAll((k) => k == null);

            if (koboldList.Count < maxKobolds)
                SpawnKobold();

            timer = Random.Range(minInterval, maxInterval);
        }
    }

    private void SpawnKobold()
    {
        Vector3 spawnPos = Quaternion.Euler(0, Random.value * 360.0f, 0) * Vector3.forward;
        spawnPos = transform.position + spawnPos.normalized * Random.Range(minRange, maxRange);
        spawnPos.y = -5;
        Quaternion spawnRotation = Quaternion.Euler(0, Random.value * 360.0f, 0);
        koboldList.Add(Instantiate(koboldPrefab, spawnPos, spawnRotation));
    }
}