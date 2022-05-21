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
    public int size = 24;

    private InventorySlot currentAbility = null;

    public InventorySlot FindItemByName(string name) {
        var col = Container.FindAll(o => o.item != null);
        if (col.Count == 0) {
            return null;
        }
        return col.Find(o => o.item.name.ToLower().Equals(name.ToLower()));
    }

    public int FindIndex(BaseItemObject _item)
    {
        return Container.FindIndex(0, Container.Count, o => o.item == _item);
    }

    public void AddItem(BaseItemObject _item, int _amount = 1) {
        foreach (InventorySlot inventorySlot in Container) {
            if (inventorySlot.item == _item) {
                inventorySlot.amount += _amount;
                return;
            }
        }

        SetEmptySlot(_item, _amount);
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
        CreateSlots();
        RepopulateInventory(InventorySO);
    }

    internal void OnPointerClick(InventorySlot _inventorySlot)
    {
        var item = _inventorySlot.item;

        switch (item.type) {
            case ItemType.Ability:
                ChangeToAbility(_inventorySlot);
                break;
            default:
                AddItem(_inventorySlot.item);
                break;
        }
    }

    public List<InventorySlot> GetAbilities() {
        var col = Container.FindAll(o => o.item != null);
        if (col.Count == 0)
        {
            return null;
        }

        var retVal = col.FindAll(o => o.item.type == ItemType.Ability);
        retVal.Sort();
        
        return retVal;
    }

    public InventorySlot CurrentAbility() {
        if (currentAbility == null) {
            currentAbility = GetAbilities()[0];
        }
        
        return currentAbility;
    }

    public void ChangeNextAbility() {
        var abilities = GetAbilities();
        var indexCur = abilities.IndexOf(currentAbility);

        currentAbility = abilities[(indexCur + 1) % abilities.Count];
    }

    public bool ChangeToAbility(InventorySlot _inventorySlot)
    {
        if (!Container.Contains(_inventorySlot)) { 
            return false;
        }

        currentAbility = _inventorySlot;
        return true;
    }

    public void ChangePrevAbility()
    {
        var abilities = GetAbilities();
        var indexCur = abilities.IndexOf(currentAbility);

        currentAbility = abilities[(indexCur - 1) % abilities.Count];
    }

    private InventorySlot SetEmptySlot(BaseItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == null)
            {
                Container[i].UpdateSlot(_item, _amount);
                return Container[i];
            }
        }

        //TODO: increase inv.size
        return null;
    }

    private void CreateSlots()
    {
        Container = new List<InventorySlot>();
        for (int i = 0; i < size; i++)
        {
            Container.Add(new InventorySlot());
        }
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
