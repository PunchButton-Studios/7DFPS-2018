using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalSlot : MonoBehaviour
{
    public Text text;
    public string floorSlotFormat = "Floor Slot #{0}";
    public string wallSlotFormat = "Wall Slot #{0}";

    public int id;
    public bool isWall;
    public TerminalUI parent;
    
    public void Setup(int id, bool isWallSlot, TerminalUI parent)
    {
        this.id = id;
        this.isWall = isWallSlot;
        this.parent = parent;

        text.text = string.Format(isWallSlot ? wallSlotFormat : floorSlotFormat, id);
    }

    public void Select() => parent.TargetSlot(id, isWall, GetComponent<Image>());
}