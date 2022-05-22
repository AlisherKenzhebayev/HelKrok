public abstract class BaseConsumableItemObject : BaseItemObject
{
    internal virtual void Awake()
    {
        type = ItemType.Consumable;
        tagName = "consumable";
    }

    public abstract void Execute();
}
