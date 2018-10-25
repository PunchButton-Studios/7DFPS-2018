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
    public Image activationProgressCircle;

    public Image batteryImage, batteryBarImage, batteryChargeImage, batteryChargeProgressImage;
    public Color[] batteryColors = new Color[4] { Color.gray, Color.red, Color.yellow, Color.green };

    public Image callHomeImage, callHomeProgressImage;
    public Color[] callHomeColors = new Color[2] { Color.red, Color.green };

    public Image timerBarImage;

    public Gradient progressCircleColorGradient;

    public Text oreResourceCountText;

    public GameObject activatePrompt, callHomePrompt, flashlightPrompt;

    public GameObject loadScreen;
    private bool loadScreenUsed = false;

    private void Update()
    {
        if (loadScreenUsed)
            return;

        loadScreen.SetActive(GameManager.isLoading);
        if (!GameManager.isLoading)
            loadScreenUsed = true;
    }

    public void UpdateGUI(Activatable targetActivatable, bool canCallHome, EntityPlayer player)
    {
        reticle.active = targetActivatable != null;
        targetTextObject.SetActive(targetActivatable != null);

        if (targetActivatable != null)
        {
            targetText.text = targetActivatable.ActivateInfo;
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetText.rectTransform);

            float activationProgress = player.activationTimer / targetActivatable.ActivationTime;
            activationProgressCircle.fillAmount = activationProgress;
            activationProgressCircle.color = progressCircleColorGradient.Evaluate(activationProgress);
        }
        else
            activationProgressCircle.fillAmount = 0.0f;

        UpdateHomeCallInfo(canCallHome, player);
        UpdateAnxiety(player.anxiety, player.maxAnxiety);
        UpdateBattery(player.energy, player.isCharging);
        UpdateTimer(player.timeLimit, player.maxTimeLimit);
        UpdateResourceList();
        UpdatePrompts(targetActivatable != null, canCallHome, player.enabled);
    }

    private void UpdateAnxiety(float anxiety, float maxAnxiety)
    {
        Color c = anxietyOverlay.color;
        c.a = anxiety / maxAnxiety;
        anxietyOverlay.color = c;
    }

    private void UpdateBattery(float energy, bool charging)
    {
        int state = (int)Mathf.Ceil(energy * 3);
        batteryImage.color = batteryBarImage.color = batteryColors[state];
        batteryBarImage.fillAmount = (state * 0.3333f);
        if (state == 1)
            batteryImage.enabled = batteryBarImage.enabled = Mathf.Round(Time.unscaledTime) % 2 == 0;
        else
            batteryImage.enabled = batteryBarImage.enabled = true;

        batteryChargeImage.enabled = charging;
        batteryChargeProgressImage.enabled = charging;

        if(charging)
        {
            batteryChargeProgressImage.fillAmount = energy;
            batteryChargeProgressImage.color = progressCircleColorGradient.Evaluate(energy);
        }
    }

    private void UpdateHomeCallInfo(bool canCallHome, EntityPlayer player)
    {
        callHomeImage.color = callHomeColors[canCallHome ? 1 : 0];
        float progress = player.homeCallTimer / player.HomeCallTime;
        callHomeProgressImage.fillAmount = progress;
        callHomeProgressImage.color = progressCircleColorGradient.Evaluate(progress);
    }

    private void UpdateResourceList()
    {
        oreResourceCountText.text = BaseController.Main.ore.ToString() + (BaseController.Main.retrievingOre > 0 ? $" +{BaseController.Main.retrievingOre.ToString()}" : string.Empty);
    }

    private void UpdatePrompts(bool hasTargetActivatable, bool canCallHome, bool playerControlEnabled)
    {
        activatePrompt.SetActive(hasTargetActivatable && playerControlEnabled);
        callHomePrompt.SetActive(canCallHome && playerControlEnabled);
        flashlightPrompt.SetActive(playerControlEnabled);
    }

    private void UpdateTimer(float time, float maxTime)
    {
        float progress = time / maxTime;
        timerBarImage.fillAmount = progress;
        timerBarImage.color = progressCircleColorGradient.Evaluate(progress);
    }
}