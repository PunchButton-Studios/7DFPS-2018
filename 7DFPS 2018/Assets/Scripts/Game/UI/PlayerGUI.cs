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
    }

    public void UpdateAnxiety(float anxiety, float maxAnxiety)
    {
        Color c = anxietyOverlay.color;
        c.a = anxiety / maxAnxiety;
        anxietyOverlay.color = c;
    }
}