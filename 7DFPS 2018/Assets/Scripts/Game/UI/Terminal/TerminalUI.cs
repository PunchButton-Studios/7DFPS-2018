using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalUI : MonoBehaviour, ICloseableMenu
{
    public ActivatableBaseTerminal terminal;
    private int slotId;
    private bool targetSlotIsWallSlot;

    public RectTransform slotContent, objectContent;

    public GameObject slotEntryPrefab, objectEntryPrefab;

    private List<GameObject> slotEntries = new List<GameObject>();
    private List<GameObject> objectEntries = new List<GameObject>();

    private Image lastSlotImage;

    private TerminalObject selectedObjectEntry;

    public Color selectedColor, unselectedColor;
    public Button createButton;

    public TerminalObject SelectedObjectEntry
    {
        get
        {
            return selectedObjectEntry;
        }
        set
        {
            if (selectedObjectEntry != null)
                selectedObjectEntry.GetComponent<Image>().color = unselectedColor;
            selectedObjectEntry = value;
            if (selectedObjectEntry != null)
            {
                selectedObjectEntry.GetComponent<Image>().color = selectedColor;
                createButton.interactable = true;
            }
            else
                createButton.interactable = false;
        }
    }

    private void OnEnable()
    {
        SetupSlotContent();
        createButton.interactable = false;
    }

    private void OnDisable()
    {
        foreach (GameObject slotEntry in slotEntries)
            Destroy(slotEntry);
        slotEntries.Clear();
        foreach (GameObject entry in objectEntries)
            Destroy(entry);
        objectEntries.Clear();
    }

    private void SetupSlotContent()
    {
        for (int i = 0; i < terminal.groundSlots.Length; i++)
            AddSlot(i, false);
        for (int i = 0; i < terminal.wallSlots.Length; i++)
            AddSlot(i, true);
    }

    private void AddSlot(int id, bool isWallSlot)
    {
        GameObject slotEntry = Instantiate(slotEntryPrefab, slotContent);
        slotEntry.GetComponent<Image>().color = unselectedColor;
        TerminalSlot slot = slotEntry.GetComponent<TerminalSlot>();
        slot.Setup(id, isWallSlot, this);
        slotEntries.Add(slotEntry);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.Main.CloseMenu();
    }

    public void TargetSlot(int id, bool isWallSlot, Image image)
    {
        if (lastSlotImage != null)
            lastSlotImage.color = unselectedColor;
        image.color = selectedColor;
        lastSlotImage = image;

        slotId = id;
        targetSlotIsWallSlot = isWallSlot;
        createButton.interactable = false;
        SetupObjectContent();
    }

    private void SetupObjectContent()
    {
        foreach (GameObject entry in objectEntries)
            Destroy(entry);
        objectEntries.Clear();

        BaseObject[] baseObjects = terminal.baseObjects.OrderBy((bo) => bo.CanAfford()).ToArray();

        BaseObject currentObject;
        if (targetSlotIsWallSlot)
            currentObject = terminal.wallSlots[slotId].BaseObject;
        else
            currentObject = terminal.groundSlots[slotId].BaseObject;

        foreach(BaseObject baseObject in baseObjects)
        {
            if (baseObject.isWallObject == targetSlotIsWallSlot)
            {
                GameObject entry = Instantiate(objectEntryPrefab, objectContent);
                TerminalObject terminalObject = entry.GetComponent<TerminalObject>();
                BaseObject targetObject = baseObject;
                bool alreadyInSlot = false;

                if (currentObject != null && currentObject.Equals(baseObject))
                {
                    SelectedObjectEntry = terminalObject;
                    targetObject = targetObject.Copy();
                    targetObject.oreCost = 0;
                    alreadyInSlot = true;
                }
                else
                    entry.GetComponent<Image>().color = unselectedColor;

                terminalObject.Setup(targetObject, this, alreadyInSlot);

                entry.GetComponent<Button>().interactable = baseObject.CanAfford();
                objectEntries.Add(entry);
            }
        }
    }

    public void CreateButton()
    {
        if (!SelectedObjectEntry.alreadyInSlot)
        {
            BaseSlot targetSlot;
            if (targetSlotIsWallSlot)
                targetSlot = terminal.wallSlots[slotId];
            else
                targetSlot = terminal.groundSlots[slotId];

            targetSlot.BaseObject = SelectedObjectEntry.baseObject;
            BaseController.Main.ore -= targetSlot.BaseObject.oreCost;
        }
        Close();
    }
}