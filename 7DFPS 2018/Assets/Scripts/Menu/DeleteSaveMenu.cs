using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveMenu : MonoBehaviour
{
    public Text text;
    public string format = "Delete {0}? It'll be gone, forever!";
    private string target;

    public void SetTarget(string target)
    {
        this.target = target;
        this.text.text = string.Format(format, target);
    }

    public void ConfirmDelete() => Directory.Delete($@"{Application.persistentDataPath}\saves\{target}", true);
}