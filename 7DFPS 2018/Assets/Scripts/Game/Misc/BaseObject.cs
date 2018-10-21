using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : ScriptableObject
{
    public GameObject prefab;
    public bool isWallObject;
    public Sprite preview;

    public int oreCost;

    public bool CanAfford() => BaseController.Main.ore >= oreCost;

    public BaseObject Copy()
    {
        BaseObject baseObject = ScriptableObject.CreateInstance<BaseObject>();
        baseObject.name = this.name;
        baseObject.prefab = this.prefab;
        baseObject.isWallObject = this.isWallObject;
        baseObject.preview = this.preview;
        baseObject.oreCost = this.oreCost;
        return baseObject;
    }

    public override bool Equals(object other)
    {
        if (other is BaseObject)
        {
            BaseObject otherObject = other as BaseObject;

            return otherObject.prefab == this.prefab && otherObject.isWallObject == this.isWallObject;
        }
        else
            return false;
    }

    public override int GetHashCode()
    {
        return prefab.GetHashCode() ^ isWallObject.GetHashCode();
    }
}