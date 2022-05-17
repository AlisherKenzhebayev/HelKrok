using UnityEngine;

public enum ItemType { 
    Pickup,
    Consumable,
    Key,
    Ability,
    Default,
}

public abstract class BaseItemObject : ScriptableObject
{
    public GameObject prefabUI;
    public ItemType type;
    public int priority;
    [TextArea(15, 2)]
    public string description;
    public string tagName;
}
