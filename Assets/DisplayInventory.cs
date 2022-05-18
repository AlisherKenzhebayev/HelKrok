using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{
    private GameObject player;
    private Inventory playerInventory;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int Y_SPACE_BETWEEN_ITEM;
    public int NUMBER_COLUMN;
    
    private Dictionary<InventorySlot, GameObject> itemDisplay;

    void Start()
    {
        itemDisplay = new Dictionary<InventorySlot, GameObject>();
        
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

        CreateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }

    private void CreateDisplay()
    {
        for(int i = 0; i < playerInventory.Container.Count; i++) {
            var obj = Instantiate(playerInventory.Container[i].item.prefabUI, Vector3.zero, Quaternion.identity, this.transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = playerInventory.Container[i].amount.ToString("x{n0}");
            itemDisplay.Add(playerInventory.Container[i], obj);
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
        for (int i = 0; i < playerInventory.Container.Count; i++)
        {
            if (itemDisplay.ContainsKey(playerInventory.Container[i]))
            {
                itemDisplay[playerInventory.Container[i]].GetComponent<RectTransform>().localPosition = GetPosition(i);
                itemDisplay[playerInventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text =
                    FormatCount(playerInventory.Container[i].amount);
            }
            else {
                var obj = Instantiate(playerInventory.Container[i].item.prefabUI, Vector3.zero, Quaternion.identity, this.transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = FormatCount(playerInventory.Container[i].amount);;
                itemDisplay.Add(playerInventory.Container[i], obj);
            }
        }
    }

    private string FormatCount(int amount)
    {
        return amount.ToString("x{n0}");
    }
}
