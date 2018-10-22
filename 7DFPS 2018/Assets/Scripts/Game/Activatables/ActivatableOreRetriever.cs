using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableOreRetriever : Activatable
{
    public override string ActivateInfo
    {
        get
        {
            return $"Retrieve {BaseController.Main.taggedOre} ore.";
        }
    }

    public override void Activate(EntityPlayer player) => BaseController.Main.collectingState = BaseController.CollectingState.CollectingTaggedOres;

    public override bool CanBeActivated(EntityPlayer player) => 
        BaseController.Main.collectingState == BaseController.CollectingState.Idle && BaseController.Main.taggedOre > 0;
}