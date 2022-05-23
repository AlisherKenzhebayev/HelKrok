using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    private GameObject player;
    private Inventory playerInventory;

    public GameObject emptySlotPrefab;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int Y_SPACE_BETWEEN_ITEM;
    public int NUMBER_COLUMN;
    
    private Dictionary<GameObject, InventorySlot> itemDisplay;

    void Start()
    {
        itemDisplay = new Dictionary<GameObject, InventorySlot>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerInventory = player.GetComponentInChildren<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Error - no playerInventory child component exists");
        }

        CreateSlots();
    }
    void Update()
    {
        UpdateDisplay();
    }

    private void CreateSlots()
    {
        Debug.Log("DiplayInventory - CreateSlots");

        itemDisplay = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < playerInventory.Container.Count; i++)
        {
            var obj = Instantiate(emptySlotPrefab, Vector3.zero, Quaternion.identity, this.transform);

            RegisterEventAction(obj, EventTriggerType.PointerClick, delegate { this.OnPointerClick(obj); });

            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            itemDisplay.Add(obj, playerInventory.Container[i]);
        }
    }

    private void OnPointerClick(GameObject obj)
    {
        var inventorySlot = itemDisplay[obj];
        if (inventorySlot.item == null) {
            return;
        }

        playerInventory.OnPointerClick(inventorySlot);
    }

    private Vector3 GetPosition(int i)
    {
        Vector3 retVal = new Vector3(
            X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_COLUMN)),
            Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_COLUMN)), 
            0f);
        return retVal;
    }

    private void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemDisplay)
        {
            if (_slot.Value.item != null)
            {
                // Rewrite the sprite, sprite color, button color, button enable
                _slot.Key.transform.GetComponent<Image>().color
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetComponent<Image>().color;

                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetChild(0).GetComponentInChildren<Image>().sprite;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetChild(0).GetComponentInChildren<Image>().color;

                _slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetChild(1).GetComponentInChildren<Image>().sprite;
                _slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().color
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetChild(1).GetComponentInChildren<Image>().color;

                _slot.Key.transform.GetChild(1).GetComponentInChildren<Button>().enabled
                    = playerInventory.FindItemByName(_slot.Value.item.name).item.prefabUI.transform.GetChild(1).GetComponentInChildren<Button>().enabled;
                
                // Update the text counter
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = FormatCount(_slot.Value.amount);
            }
            else {
                _slot.Key.transform.GetComponent<Image>().color
                        = emptySlotPrefab.transform.GetComponent<Image>().color;

                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite
                    = emptySlotPrefab.transform.GetChild(0).GetComponentInChildren<Image>().sprite;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color
                    = emptySlotPrefab.transform.GetChild(0).GetComponentInChildren<Image>().color;

                _slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().sprite
                    = emptySlotPrefab.transform.GetChild(1).GetComponentInChildren<Image>().sprite;
                _slot.Key.transform.GetChild(1).GetComponentInChildren<Image>().color
                    = emptySlotPrefab.transform.GetChild(1).GetComponentInChildren<Image>().color;

                _slot.Key.transform.GetChild(1).GetComponentInChildren<Button>().enabled
                    = emptySlotPrefab.transform.GetChild(1).GetComponentInChildren<Button>().enabled;

                // Update the text counter
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    private void RegisterEventAction(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> unityAction) {
        var eventTrigger = obj.GetComponentInChildren<EventTrigger>();
        var trigger = new EventTrigger.Entry();
        trigger.eventID = type;
        trigger.callback.AddListener(unityAction);
        eventTrigger.triggers.Add(trigger);
    }

    private string FormatCount(int amount)
    {
        return "x" + amount.ToString("n0");
    }
}
