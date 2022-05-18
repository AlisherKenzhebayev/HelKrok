using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Any inventory holding script class, repopulates the inventory from a memory slot + InventorySO (with typeLoad) at the start.
/// If none is given, just restore the saved/memory inventory.
/// </summary>
public class Inventory : MonoBehaviour
{
    public InventoryObject InventorySO = null;
    public List<InventorySlot> Container;

    public bool FindItemName(string name) {
        return Container.Exists(o => o.item.name.ToLower().Equals(name.ToLower()));
    }

    public void AddItem(BaseItemObject _item, int _amount = 1) {
        foreach (InventorySlot inventorySlot in Container) {
            if (inventorySlot.item == _item) {
                inventorySlot.amount += _amount;
                return;
            }
        }

        Container.Add(new InventorySlot(_item, _amount));
        Container.Sort();
    }

    public void RemoveItem(BaseItemObject _item, int _amount = 1)
    {
        foreach (InventorySlot inventorySlot in Container)
        {
            if (inventorySlot.item == _item)
            {
                inventorySlot.amount = Math.Max(0, inventorySlot.amount - _amount);
                return;
            }
        }

        Container.Add(new InventorySlot(_item, _amount));
    }

    public int GetItemCount(BaseItemObject _item) {
        if (!Container.Exists(o => o.item == _item)) {
            return 0;
        }

        var retval = Container.Find(o => o.item == _item);
        return retval.amount;
    }

    private void Start()
    {
        Container = new List<InventorySlot>();

        RepopulateInventory(InventorySO);
    }

    private void RepopulateInventory(InventoryObject _inventoryObject)
    {
        if (_inventoryObject == null) {
            return;
        }
        
        // TODO: load from memory/save


        // Adds the existing referenced SO data accordingly
        switch (_inventoryObject.LoadingType)
        {
            default:
                AddMinimalExclusive(_inventoryObject.Container);
                break;
        }
    }

    /// <summary>
    /// Adds items as referenced in the InventorySO, only in case this Container has less/has no items like that already.
    /// </summary>
    private void AddMinimalExclusive(List<InventorySlot> _container)
    {
        foreach (InventorySlot inventorySlot in _container) 
        {
            var _item = inventorySlot.item;
            var _amount = inventorySlot.amount;

            var _curAmount = GetItemCount(_item);
            if (_curAmount >= _amount) {
                continue;
            }

            var _diffAmount = _amount - _curAmount;
            AddItem(_item, _diffAmount);
        }
    }
}
