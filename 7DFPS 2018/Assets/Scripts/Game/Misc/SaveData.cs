using System;
using System.IO;
using UnityEngine;

public class SaveData
{
    public Metadata metadata = new Metadata();
    public WorldData worldData = new WorldData();
    public BaseData baseData = new BaseData();
    public PlayerData playerData = new PlayerData();

    public void Save(DirectoryInfo directory)
    {
        string playerJson = JsonUtility.ToJson(playerData, Application.isEditor);
        string playerDataPath = $@"{directory.ToString()}\player.json";
        File.WriteAllText(playerDataPath, playerJson);

        string baseJson = JsonUtility.ToJson(baseData, Application.isEditor);
        string baseDataPath = $@"{directory.ToString()}\base.json";
        File.WriteAllText(baseDataPath, baseJson);

        string worldJson = JsonUtility.ToJson(worldData, Application.isEditor);
        string worldDataPath = $@"{directory.ToString()}\world.json";
        File.WriteAllText(worldDataPath, worldJson);

        metadata.saveName = GameManager.saveName;
        metadata.lastSave = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        string metadataJson = JsonUtility.ToJson(metadata);
        File.WriteAllText($@"{directory.ToString()}\meta.json", metadataJson);
    }

    public static SaveData Load(DirectoryInfo directory)
    {
        SaveData saveData = new SaveData();

        string playerDataPath = $@"{directory.ToString()}\player.json";
        if (File.Exists(playerDataPath))
        {
            string playerJson = File.ReadAllText(playerDataPath);
            saveData.playerData = JsonUtility.FromJson<PlayerData>(playerJson);
        }
        else
            saveData.playerData = new PlayerData();

        string baseDataPath = $@"{directory.ToString()}\base.json";
        if (File.Exists(baseDataPath))
        {
            string baseJson = File.ReadAllText(baseDataPath);
            saveData.baseData = JsonUtility.FromJson<BaseData>(baseJson);
        }
        else
            saveData.baseData = new BaseData();

        string worldDataPath = $@"{directory.ToString()}\world.json";
        if (File.Exists(worldDataPath))
        {
            string worldJson = File.ReadAllText(worldDataPath);
            saveData.worldData = JsonUtility.FromJson<WorldData>(worldJson);
        }
        else
            saveData.worldData = new WorldData();

        return saveData;
    }

    public class PlayerData
    {
        public float anxiety = 0.0f;
        public float energy = 0.0f;
        public Vector3 position = new Vector3(0, 0, 0);
        public Quaternion rotation = Quaternion.identity;
    }

    public class BaseData
    {
        public Vector3 position = new Vector3(0, 0, 0);

        public int ore = 0;
        public ObjectData[] wallObjects = new ObjectData[0];
        public ObjectData[] groundObjects = new ObjectData[0];

        [Serializable]
        public class ObjectData
        {
            public int id;
            public string extraData;
        }
    }

    public class WorldData
    {
        public byte[] map = new byte[0];
        public byte[] extraData = new byte[0];
        public int mapSize = 0;

        public void SaveExtraData(IWorldGenObject worldGenObject, byte data)
        {
            int id = worldGenObject.Id;
            if (id >= extraData.Length)
            {
                byte[] newEDArray = new byte[id + 1];
                Array.Copy(extraData, newEDArray, extraData.Length);
                extraData = newEDArray;
            }
            extraData[id] = data;
        }
    }

    public class Metadata
    {
        public string saveName;
        public long lastSave;
    }
}