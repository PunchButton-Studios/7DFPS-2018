using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public abstract string ActivateInfo { get; }

    public abstract void Activate(EntityPlayer player);

    public virtual bool CanBeActivated(EntityPlayer player)
    {
        return true;
    }
}