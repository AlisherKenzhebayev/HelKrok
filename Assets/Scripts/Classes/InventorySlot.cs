using System;

[System.Serializable]
public class InventorySlot : IComparable
{
    public BaseItemObject item;
    public int amount;

    public InventorySlot()
    {
        item = null;
        amount = 0;
    }

    public InventorySlot(BaseItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public int CompareTo(object obj)
    {
        if (this.item == null) return 1;
        if (obj == null) return -1;

        InventorySlot otherItem = obj as InventorySlot;
        if (otherItem.item == null) return -1;
        
        if (otherItem != null)
        {
            return this.item.CompareTo(otherItem.item);
        }
        else
        {
            throw new ArgumentException("Object is not a InventorySlot");
        }
    }

    internal void UpdateSlot(BaseItemObject _item, int _amount)
    {
        this.item = _item;
        this.amount = _amount;
    }
}