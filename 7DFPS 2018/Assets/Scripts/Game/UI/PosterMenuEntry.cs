using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PosterMenuEntry : MonoBehaviour
{
    public PosterMenu parent;
    public Text text;
    public string filepath;

    public void Setup(FileInfo fileInfo, PosterMenu parent)
    {
        this.parent = parent;
        text.text = fileInfo.Name;
        filepath = fileInfo.ToString();
    }

    public void Select() => parent.Select(filepath);
}