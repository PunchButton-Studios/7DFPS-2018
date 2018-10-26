using System.IO;
using UnityEngine;

public class Config
{
    public float volume = 0.6f;
    public bool vsync = false;
    public bool buttonPrompts = true;

    public Keybinds keybinds = new Keybinds();

    private static Config main;
    public static Config Main
    {
        get
        {
            if (main == null)
                Load();
            return main;
        }
    }

    public static void Apply()
    {
        AudioListener.volume = Main.volume;
        QualitySettings.vSyncCount = Main.vsync ? 1 : 0;
    }

    public static void Load()
    {
        string path = $@"{Application.persistentDataPath}\config.json";
        if (File.Exists(path))
            main = JsonUtility.FromJson<Config>(File.ReadAllText(path));
        else
            main = new Config();
    }

    public static void Save()
    {
        string path = $@"{Application.persistentDataPath}\config.json";
        File.WriteAllText(path, JsonUtility.ToJson(Main));
    }

    [System.Serializable]
    public class Keybinds
    {
        public KeyCode movementForward = KeyCode.W, movementBackward = KeyCode.S;
        public KeyCode movementLeft = KeyCode.A, movementRight = KeyCode.D;
        public KeyCode action = KeyCode.Mouse0;
        public KeyCode flashlight = KeyCode.F;
        public KeyCode pause = KeyCode.Escape;
        public KeyCode callHome = KeyCode.H;
        public KeyCode sonar = KeyCode.G;
    }
}