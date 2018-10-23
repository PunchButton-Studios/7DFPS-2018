using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public float activationTime = 0.01f;
    public abstract string ActivateInfo { get; }
    public virtual float ActivationTime { get { return activationTime; } }

    public abstract void Activate(EntityPlayer player);

    public virtual bool CanBeActivated(EntityPlayer player)
    {
        return true;
    }
}