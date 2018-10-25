using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PosterMenu : MonoBehaviour, ICloseableMenu
{
    private ActivatablePoster poster;
    public GameObject canvas;
    public RectTransform content;
    public GameObject entryPrefab;
    private List<GameObject> entries = new List<GameObject>();

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            SetupList();
    }

    public void Open(ActivatablePoster poster)
    {
        this.poster = poster;
        canvas.SetActive(true);
        SetupList();
        GameManager.Main.OpenMenu(this);
    }

    public void Close()
    {
        ClearList();
        canvas.SetActive(false);
        GameManager.Main.CloseMenu();
    }

    public void OpenFolder()
    {
        string path = $@"{Application.streamingAssetsPath}\poster";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        System.Diagnostics.Process.Start(path);
    }

    private void ClearList()
    {
        foreach (GameObject entry in entries)
            Destroy(entry);
        entries.Clear();
    }

    private void SetupList()
    {
        ClearList();
        string path = $@"{Application.streamingAssetsPath}\poster";
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
            directoryInfo.Create();

        SetupList(directoryInfo, "*.png");
        SetupList(directoryInfo, "*.jpg");
    }

    private void SetupList(DirectoryInfo directoryInfo, string searchPattern)
    {
        FileInfo[] fileInfos = directoryInfo.GetFiles(searchPattern);
        foreach (FileInfo fileInfo in fileInfos)
        {
            GameObject entry = Instantiate(entryPrefab, content);
            PosterMenuEntry menuEntry = entry.GetComponent<PosterMenuEntry>();
            menuEntry.Setup(fileInfo, this);
            entries.Add(entry);
        }
    }

    public void Select(string filepath)
    {
        poster.ApplyExtraData(filepath);
        Close();
    }
}