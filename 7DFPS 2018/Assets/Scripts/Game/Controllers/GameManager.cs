using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string PAUSE_BUTTON_NAME = "Pause";

    private bool isPaused = false;
    private ICloseableMenu activeMenu;
    private float oldTimescale = 1.0f;

    public static string saveName = "New Save";
    public static StartState startState = StartState.NewGame;

    private static GameManager main;

    public event OnPause pauseEvent;
    public event AccessSaveData loadGameEvent;
    public event AccessSaveData saveGameEvent;

    public static GameManager Main
    {
        get
        {
            return main ?? (main = new GameObject("Game Manager").AddComponent<GameManager>());
        }
    }

    public static bool GamePaused
    {
        get
        {
            return Main.isPaused;
        }
        set
        {
            Main.Pause(value);
        }
    }

    public ICloseableMenu ActiveMenu
    {
        get
        {
            return activeMenu;
        }
    }

    private void Start()
    {
        if (startState == StartState.LoadGame)
            LoadGame();
        else if (startState == StartState.NewGame)
            FindObjectOfType<WorldGenerator>().GenerateLevel(100, new Vector2Int(3,3), true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.AddComponent<DebugHandler>();
    }

    public void CloseMenu()
    {
        activeMenu = null;
        GamePaused = false;
    }

    public void OpenMenu(ICloseableMenu closeableMenu)
    {
        activeMenu = closeableMenu;
        GamePaused = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown(PAUSE_BUTTON_NAME))
        {
            if(activeMenu != null)
                activeMenu.Close();
            else
            {
                PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
                pauseMenu.canvas.SetActive(true);
                OpenMenu(pauseMenu);
            }
        }
    }

    private void OnDestroy()
    {
        main = null;
    }

    public void Pause(bool pauseState)
    {
        if (pauseState == isPaused)
            return; //Ignore

        if (pauseState)
            oldTimescale = Time.timeScale;
        Time.timeScale = pauseState ? 0.0f : oldTimescale;
        Cursor.lockState = pauseState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pauseState;

        isPaused = pauseState;
        pauseEvent?.Invoke(pauseState);
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        DirectoryInfo directoryInfo = new DirectoryInfo($@"{Application.persistentDataPath}\saves\{saveName}");
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        saveGameEvent?.Invoke(saveData);
        saveData.Save(directoryInfo);
    }

    public void LoadGame()
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($@"{Application.persistentDataPath}\saves\{saveName}");
            SaveData saveData = SaveData.Load(directoryInfo);
            loadGameEvent?.Invoke(saveData);
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Failed to load game. Is the save corrupt?\r\n" + ex.ToString());
        }
    }

    public enum StartState
    {
        NewGame,
        LoadGame,
    }
}