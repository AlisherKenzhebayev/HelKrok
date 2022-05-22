using UnityEngine;

public abstract class BaseAbilityItemObject : BaseItemObject
{
    internal virtual void Awake()
    {
        type = ItemType.Ability;
        tagName = "ability";
    }

    public abstract void Execute(GameObject _gameObject, bool _enable, Transform _transform);
}
