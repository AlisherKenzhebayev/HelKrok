using UnityEngine;

public abstract class BaseAbilityItemObject : BaseItemObject
{
    public float energyCost = 3f;
    public float continuousEnergyCost = 0.5f;

    internal virtual void Awake()
    {
        type = ItemType.Ability;
        tagName = "ability";
    }

    public virtual float GetEnergyCost()
    {
        return energyCost;
    }

    public virtual float GetContEnergyCost()
    {
        return continuousEnergyCost;
    }


    public abstract void Execute(GameObject _gameObject, bool _enable, Transform _transform);
}
