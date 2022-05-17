[System.Serializable]
public class InventorySlot
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
}