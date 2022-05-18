using System;
using UnityEngine;

public enum ItemType { 
    Pickup,
    Consumable,
    Key,
    Ability,
    Default,
}

public abstract class BaseItemObject : ScriptableObject, IComparable
{
    public GameObject prefabUI;
    public ItemType type;
    public int priority;
    [TextArea(15, 2)]
    public string description;
    public string tagName;

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        BaseItemObject otherItem = obj as BaseItemObject;
        if (otherItem != null)
            return -1 * this.priority.CompareTo(otherItem.priority);
        else
            throw new ArgumentException("Object is not a BaseItemObject");
    }
}
