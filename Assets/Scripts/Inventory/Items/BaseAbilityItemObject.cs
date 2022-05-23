using UnityEngine;

public abstract class BaseAbilityItemObject : BaseItemObject
{
    public float energyCost = 3f;
    public float continuousEnergyCost = 0.5f;

    public float timerCooldown = 2f;

    // TODO: Add cooldown on press/execute, similar to the one in the coroutine for projectile spawner. Otherwise -> desync

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


    public abstract bool Execute(GameObject _gameObject, bool _enable, Transform _transform);
}
