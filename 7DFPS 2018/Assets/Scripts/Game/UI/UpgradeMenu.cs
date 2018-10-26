using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour, ICloseableMenu
{
    public Button createButton;
    public EntityPlayer entityPlayer;

    public Button sonarUpgrade, compassUpgrade;
    public int sonarCost, compassCost;

    private Upgrade targetUpgrade;

    public Color selectColor, unselectColor;

    private void OnEnable()
    {
        createButton.interactable = false;
        sonarUpgrade.interactable = !entityPlayer.hasSonar && BaseController.Main.ore >= sonarCost;
        compassUpgrade.interactable = !entityPlayer.hasCompass && BaseController.Main.ore >= compassCost;
    }

    public void TargetSonar() => SetUpgradeTarget(Upgrade.Sonar);
    public void TargetCompass() => SetUpgradeTarget(Upgrade.Compass);

    public void SetUpgradeTarget(Upgrade upgrade)
    {
        targetUpgrade = upgrade;
        sonarUpgrade.GetComponent<Image>().color = upgrade == Upgrade.Sonar ? selectColor : unselectColor;

        createButton.interactable = upgrade != Upgrade.None;
    }

    public void CreateButton()
    {
        switch(targetUpgrade)
        {
            case Upgrade.Sonar:
                AddSonar();
                break;
            case Upgrade.Compass:
                AddCompass();
                break;
        }

        SetUpgradeTarget(Upgrade.None);
    }

    private void AddSonar()
    {
        entityPlayer.hasSonar = true;
        BaseController.Main.ore -= sonarCost;
        sonarUpgrade.interactable = false;
    }

    private void AddCompass()
    {
        entityPlayer.hasCompass = true;
        BaseController.Main.ore -= compassCost;
        compassUpgrade.interactable = false;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.Main.CloseMenu();
    }

    public enum Upgrade
    {
        None,
        Sonar,
        Compass,
    }
}