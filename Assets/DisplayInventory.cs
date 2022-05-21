using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        itemDisplay = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < playerInventory.Container.Count; i++)
        {
            var obj = Instantiate(emptySlotPrefab, Vector3.zero, Quaternion.identity, this.transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            itemDisplay.Add(obj, playerInventory.Container[i]);
        }
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
            if (_slot.Value.item != null) {
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
            }
        }

    //    for (int i = 0; i < playerInventory.Container.Count; i++)
    //    {
    //        if (itemDisplay.ContainsKey(playerInventory.Container[i]))
    //        {
    //            itemDisplay[playerInventory.Container[i]].GetComponent<RectTransform>().localPosition = GetPosition(i);
    //            itemDisplay[playerInventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text =
    //                FormatCount(playerInventory.Container[i].amount);
    //        }
    //        else {
    //            var obj = Instantiate(playerInventory.Container[i].item.prefabUI, Vector3.zero, Quaternion.identity, this.transform);
    //            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
    //            obj.GetComponentInChildren<TextMeshProUGUI>().text = FormatCount(playerInventory.Container[i].amount);;
    //            itemDisplay.Add(playerInventory.Container[i], obj);
    //        }
    //    }
    }

    private string FormatCount(int amount)
    {
        return "x" + amount.ToString("n0");
    }
}
