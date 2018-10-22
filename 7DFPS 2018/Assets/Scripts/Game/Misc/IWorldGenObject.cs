using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldGenObject
{
    int Id { get; set; }
    void OnWorldLoaded(byte saveData);
}