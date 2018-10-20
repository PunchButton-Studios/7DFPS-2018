using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightSource
{
    float Range { get; }
    float Power { get; }
}
