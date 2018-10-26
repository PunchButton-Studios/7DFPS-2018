using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRelaxationObject : MonoBehaviour, IRelaxationObject
{
    public float anxietyDecreaseAmount;
    public float AnxietyDecreaseAmount
    {
        get
        {
            return anxietyDecreaseAmount;
        }
    }
}