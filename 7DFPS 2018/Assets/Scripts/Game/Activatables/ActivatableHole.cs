using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableHole : Activatable, ISonarResponder
{
    public Vector2Int tilePos;
    public AudioSource sonarSfx;

    public override string ActivateInfo
    {
        get
        {
            return "Head Down";
        }
    }

    public AudioSource SonarResponseSource
    {
        get
        {
            return sonarSfx;
        }
    }

    public override void Activate(EntityPlayer player) => player.GetComponent<PlayerDepthTransition>().TransferDown(tilePos, transform.position);
}