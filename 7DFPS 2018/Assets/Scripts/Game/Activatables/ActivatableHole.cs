using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableHole : Activatable
{
    public Vector2Int tilePos;

    public override string ActivateInfo
    {
        get
        {
            return "Head Down";
        }
    }

    public override void Activate(EntityPlayer player) => player.GetComponent<PlayerDepthTransition>().TransferDown(tilePos, transform.position);
}