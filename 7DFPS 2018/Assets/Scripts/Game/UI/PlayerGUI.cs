﻿using System.Collections;
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

    public Image faderImage;
    public AnimationCurve faderImageCurve;

    public Image batteryImage, batteryBarImage, batteryChargeImage, batteryChargeProgressImage;
    public Color[] batteryColors = new Color[4] { Color.gray, Color.red, Color.yellow, Color.green };

    public Image callHomeImage, callHomeProgressImage;
    public Color[] callHomeColors = new Color[2] { Color.red, Color.green };

    public Image timerBarImage;

    public Image sonarImage, sonarBarImage;

    public GameObject compassImageObject;
    public RawImage compassImage;
    public float compassXUVOffset = 0.125f;

    public RawImage depthMeterImage;
    public Text depthMeterText;
    public float depthMeterSpeed = 5;

    public Gradient progressCircleColorGradient;

    public Text oreResourceCountText;

    public GameObject activatePrompt, callHomePrompt, flashlightPrompt, sonarPrompt;

    public GameObject loadScreen;
    private bool loadScreenUsed = false;

    private void Update()
    {
        depthMeterText.text = $"-{(GameManager.Main.depth + 1) * 100}m";

        if (GameManager.GamePaused)
        {
            EntityPlayer player = FindObjectOfType<EntityPlayer>();
            UpdateCompass(player.hasCompass, player.transform.rotation.eulerAngles.y);
        }

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
        UpdateSonarInfo(player.hasSonar, player.sonarCooldown, player.sonarCooldownMax);
        UpdateCompass(player.hasCompass, player.transform.rotation.eulerAngles.y);
        UpdateResourceList();
        UpdatePrompts(targetActivatable != null, canCallHome, player.enabled, player.hasSonar);
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

    private void UpdatePrompts(bool hasTargetActivatable, bool canCallHome, bool playerControlEnabled, bool sonarEnabled)
    {
        activatePrompt.SetActive(hasTargetActivatable && playerControlEnabled);
        callHomePrompt.SetActive(canCallHome && playerControlEnabled);
        flashlightPrompt.SetActive(playerControlEnabled);
        sonarPrompt.SetActive(sonarEnabled && playerControlEnabled);
    }

    private void UpdateTimer(float time, float maxTime)
    {
        float progress = time / maxTime;
        timerBarImage.fillAmount = progress;
        timerBarImage.color = progressCircleColorGradient.Evaluate(progress);

        Color faderColor = faderImage.color;
        faderColor.a = faderImageCurve.Evaluate(1 - progress);
        faderImage.color = faderColor;
    }

    private void UpdateSonarInfo(bool hasSonar, float sonarCooldown, float sonarCooldownMax)
    {
        sonarImage.enabled = hasSonar;
        sonarBarImage.enabled = hasSonar;

        float progress = sonarCooldown / sonarCooldownMax;
        progress = Mathf.Clamp01(progress);
        sonarBarImage.fillAmount = progress;
        sonarBarImage.color = progressCircleColorGradient.Evaluate(1 - progress);
        sonarImage.color = callHomeColors[sonarCooldown < 0.0f ? 1 : 0];
    }

    private void UpdateCompass(bool hasCompass, float playerYAngle)
    {
        compassImageObject.SetActive(hasCompass);
        if (!hasCompass)
            return;

        float anglePercentage = playerYAngle / 360.0f;
        Rect uvRect = compassImage.uvRect;
        uvRect.x = anglePercentage + compassXUVOffset;
        compassImage.uvRect = uvRect;
    }

    public void ScrollDepthMeter()
    {
        Rect uvRect = depthMeterImage.uvRect;
        uvRect.y += depthMeterSpeed * Time.unscaledDeltaTime;
        depthMeterImage.uvRect = uvRect;
        depthMeterText.text = $"-{Random.Range(100, 999)}m";
    }
}