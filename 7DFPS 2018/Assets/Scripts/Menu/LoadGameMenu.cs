using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{
    public GameObject menuEntryPrefab;
    public RectTransform content;
    private List<SaveMenuEntry> saveMenuEntries = new List<SaveMenuEntry>();
    private SaveMenuEntry selectedEntry;
    public Color selectedEntryColor, unselectedEntryColor;

    public Button loadButton;

    public SaveMenuEntry SelectedEntry
    {
        get
        {
            return selectedEntry;
        }
        set
        {
            if (selectedEntry != null)
                selectedEntry.GetComponent<Image>().color = unselectedEntryColor;

            selectedEntry = value;

            if (selectedEntry != null)
                selectedEntry.GetComponent<Image>().color = selectedEntryColor;

            loadButton.interactable = selectedEntry != null;
        }
    }

    private void OnDisable()
    {
        SelectedEntry = null;
        foreach (SaveMenuEntry saveMenuEntry in saveMenuEntries)
            Destroy(saveMenuEntry.gameObject);
        saveMenuEntries.Clear();
    }

    private void OnEnable()
    {
        DirectoryInfo saveDirectory = new DirectoryInfo($@"{Application.persistentDataPath}\saves");
        DirectoryInfo[] subDirectories = saveDirectory.GetDirectories();

        List<SaveData.Metadata> metadatas = new List<SaveData.Metadata>();
        foreach(DirectoryInfo subDir in subDirectories)
        {
            string metaFilePath = $@"{subDir.ToString()}\meta.json";
            if (File.Exists(metaFilePath))
            {
                try
                {
                    metadatas.Add(JsonUtility.FromJson<SaveData.Metadata>(File.ReadAllText(metaFilePath)));
                }
                catch { }
            }
        }

        SaveData.Metadata[] saveDatas = metadatas.OrderByDescending((m) => m.lastSave).ToArray();
        foreach(SaveData.Metadata metadata in saveDatas)
        {
            GameObject entry = Instantiate(menuEntryPrefab, content);
            SaveMenuEntry saveMenuEntry = entry.GetComponent<SaveMenuEntry>();
            saveMenuEntry.SetInfo(metadata.saveName, metadata.lastSave);
            saveMenuEntries.Add(saveMenuEntry);
            saveMenuEntry.GetComponent<Image>().color = unselectedEntryColor;
        }
    }

    public void LoadButton() => FindObjectOfType<MainMenu>().LoadGame(SelectedEntry.saveNameText.text);
}