using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player/Any inventory holding script class, repopulates the inventory from a memory slot + InventorySO (with typeLoad) at the start.
/// If none is given, just restore the saved/memory inventory.
/// </summary
[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public InventoryLoadingType LoadingType;

    public bool FindItemName(string name)
    {
        return Container.Exists(o => o.item.name.Equals(name));
    }

    public void AddItem(BaseItemObject _item, int _amount)
    {
        foreach (InventorySlot inventorySlot in Container)
        {
            if (inventorySlot.item == _item)
            {
                inventorySlot.amount += _amount;
                return;
            }
        }

        Container.Add(new InventorySlot(_item, _amount));
    }

    public int GetItemCount(BaseItemObject _item)
    {
        var retval = Container.Find(o => o.item == _item);
        return retval.amount;
    }
}

public enum InventoryLoadingType { 
    Default,
    Additive,
    Minimal,
}