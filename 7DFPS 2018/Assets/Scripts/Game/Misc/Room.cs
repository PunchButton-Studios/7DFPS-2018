using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : ScriptableObject
{
    public GameObject roomPrefab;
    public RoomSize size;
    [Range(1,100)] public int weight = 50;
    [Flags] public Sides doorways;
    public bool canPlaceBase, isEndPoint;
    
    public enum Sides
    {
        North   = 0x1,
        West    = 0x2,
        East    = 0x4,
        South   = 0x8,
    }

    public enum RoomSize
    {
        Small, Big
    }
}