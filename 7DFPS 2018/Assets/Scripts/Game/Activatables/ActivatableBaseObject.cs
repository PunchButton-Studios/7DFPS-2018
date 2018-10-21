using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableBaseObject : Activatable
{
    public BaseSlot Slot
    {
        get
        {
            return GetComponentInParent<BaseSlot>();
        }
    }

    public abstract void ApplyExtraData(string extraData);
}