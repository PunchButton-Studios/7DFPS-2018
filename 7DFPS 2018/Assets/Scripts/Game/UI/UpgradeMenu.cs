using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour, ICloseableMenu
{
    public Button createButton;
    public EntityPlayer entityPlayer;

    public Button sonarUpgrade;
    public int sonarCost;

    private Upgrade targetUpgrade;

    public Color selectColor, unselectColor;

    private void OnEnable()
    {
        createButton.interactable = false;
        sonarUpgrade.interactable = !entityPlayer.hasSonar && BaseController.Main.ore >= sonarCost;
    }

    public void TargetSonar() => SetUpgradeTarget(Upgrade.Sonar);

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
        }

        SetUpgradeTarget(Upgrade.None);
    }

    public void AddSonar()
    {
        entityPlayer.hasSonar = true;
        BaseController.Main.ore -= sonarCost;
        sonarUpgrade.interactable = false;
        targetUpgrade = Upgrade.None;
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
    }
}