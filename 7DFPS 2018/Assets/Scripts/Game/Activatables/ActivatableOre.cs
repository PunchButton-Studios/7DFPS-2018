using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableOre : Activatable
{
    public string activateText = "Tag Ore";
    public GameObject tagObjectPrefab;
    private bool tagged;

    public override string ActivateInfo
    {
        get
        {
            return activateText;
        }
    }

    public override void Activate(EntityPlayer player)
    {
        if(!tagged)
        {
            tagged = true;
            Instantiate(tagObjectPrefab, transform.position, transform.rotation);
        }
    }

    public override bool CanBeActivated(EntityPlayer player) => !tagged;
}