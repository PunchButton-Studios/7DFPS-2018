using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableUpgradeTerminal : Activatable
{
    public GameObject terminalCanvas;

    public override string ActivateInfo
    {
        get
        {
            return "Access Upgrades";
        }
    }

    public override void Activate(EntityPlayer player)
    {
        terminalCanvas.SetActive(true);
        GameManager.Main.OpenMenu(terminalCanvas.GetComponent<ICloseableMenu>());
    }
}