using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public Reticle reticle;
    public GameObject targetTextObject;
    public Text targetText;
    public Image anxietyOverlay;

    public Image batteryImage, batteryBarImage;
    public Color[] batteryColors = new Color[4] { Color.gray, Color.red, Color.yellow, Color.green };

    public void UpdateGUI(Activatable targetActivatable, EntityPlayer player)
    {
        reticle.active = targetActivatable != null;
        targetTextObject.SetActive(targetActivatable != null);

        if (targetActivatable != null)
        {
            targetText.text = targetActivatable.ActivateInfo;
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetText.rectTransform);
        }

        UpdateAnxiety(player.anxiety, player.maxAnxiety);
        UpdateBattery(player.energy);
    }

    private void UpdateAnxiety(float anxiety, float maxAnxiety)
    {
        Color c = anxietyOverlay.color;
        c.a = anxiety / maxAnxiety;
        anxietyOverlay.color = c;
    }

    private void UpdateBattery(float energy)
    {
        int state = (int)Mathf.Ceil(energy * 3);
        batteryImage.color = batteryBarImage.color = batteryColors[state];
        batteryBarImage.fillAmount = (state * 0.3333f);
        if (state == 1)
            batteryImage.enabled = batteryBarImage.enabled = Mathf.Round(Time.unscaledTime) % 2 == 0;
        else
            batteryImage.enabled = batteryBarImage.enabled = true;
    }
}