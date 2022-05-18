using UnityEngine;

public abstract class BaseAbilityItemObject : BaseItemObject
{
    public virtual void Awake()
    {
        type = ItemType.Ability;
        tagName = "ability";
    }

    public abstract void Execute(GameObject go, bool enable);
}
