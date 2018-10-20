using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuEntry : MonoBehaviour
{
    public Text saveNameText;
    public Text saveDateText;

    public void SetInfo(string saveName, long saveDate)
    {
        saveNameText.text = saveName;
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(saveDate).ToLocalTime().DateTime;
        saveDateText.text = dateTime.ToString();
    }

    public void Select() => FindObjectOfType<LoadGameMenu>().SelectedEntry = this;
}