using System;

[System.Serializable]
public class InventorySlot : IComparable
{
    public BaseItemObject item;
    public int amount;

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

        if (obj == null) return 1;

        InventorySlot otherItem = obj as InventorySlot;
        if (otherItem != null)
            return this.item.CompareTo(otherItem.item);
        else
            throw new ArgumentException("Object is not a InventorySlot");
    }
}