using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour, ILightSource
{
    public float range;
    [Range(0, 1)] public float power;

    public float Range
    {
        get
        {
            return range;
        }
    }

    public float Power
    {
        get
        {
            return power;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
