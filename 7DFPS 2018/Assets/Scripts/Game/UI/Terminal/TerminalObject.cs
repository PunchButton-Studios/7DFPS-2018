using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalObject : MonoBehaviour
{
    public BaseObject baseObject;
    private TerminalUI parent;
    public bool alreadyInSlot;

    public GameObject oreCost;
    public Text oreCostText;

    public Image previewImage;
    public Text nameText;

    public void Setup(BaseObject baseObject, TerminalUI parent, bool alreadyInSlot)
    {
        this.baseObject = baseObject;
        this.parent = parent;
        this.alreadyInSlot = alreadyInSlot;

        if (baseObject.oreCost == 0)
            oreCost.SetActive(false);
        else
            oreCostText.text = baseObject.oreCost.ToString();

        previewImage.sprite = baseObject.preview;
        nameText.text = baseObject.name;
    }

    public void Select() => parent.SelectedObjectEntry = this;
}