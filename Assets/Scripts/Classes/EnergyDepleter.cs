using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For now only has current and maxEnergy
/// </summary>
public abstract class EnergyDepleter : MonoBehaviour, IEnergyDepleter
{
    [SerializeField]
    [Tooltip("Same as maxHealth, works with that assumption")]
    internal float currentEnergy = 100;
    internal float maxEnergy;

    [SerializeField]
    private float timeRebound = 1f;
    [SerializeField]
    private float defaultRestoreSpeed = 0.1f;

    private float currentRestoreSpeed;
    private float currentTimeRebound;

    private void FixedUpdate()
    {
        UpdateTimers();
        
        if(currentTimeRebound <= 0) { 
            currentEnergy = Mathf.Clamp(currentEnergy + currentRestoreSpeed, 0f, maxEnergy);
        }
    }

    private void UpdateTimers()
    {
        currentTimeRebound = Mathf.Max(currentTimeRebound - Time.fixedDeltaTime, 0f);

        if (currentTimeRebound <= 0)
        {
            currentRestoreSpeed = defaultRestoreSpeed;
        }
    }

    private void Start()
    {
        maxEnergy = currentEnergy;
    }

    internal virtual void OnEnable()
    {
        EventManager.StartListening("restoreEnergy" + this.gameObject.GetInstanceID(), OnRestoreEnergy);
    }

    internal virtual void OnDisable()
    {
        EventManager.StopListening("restoreEnergy" + this.gameObject.GetInstanceID(), OnRestoreEnergy);
    }

    private void OnRestoreEnergy(Dictionary<string, object> obj)
    {
        Debug.Log("Replenished energy");

        RestoreFlat((float)obj["amount"]);
    }

    public void ChangeRestoreSpeed(float regenSpeed)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool HasEnough(float energyCost)
    {
        if (currentEnergy >= energyCost) {
            return true;
        }

        return false;
    }

    public void RestoreFlat(float energyAmount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyAmount);
    }

    /// <summary>
    /// Do something, uses energy. Sets the rebound timer anew.
    /// </summary>
    public virtual bool Use(float energyAmount = 25f, float percentagePrice = 0.2f)
    {
        if (!HasEnough(energyAmount)) {
            return false;
        }

        // Otherwise already has enough energy, uses it for something and has to payPrice

        PayPrice(percentagePrice);
        return true;
    }

    internal virtual void PayPrice(float percentagePrice)
    {
        currentEnergy = Mathf.Max(0f, currentEnergy * (1 - percentagePrice));
        currentTimeRebound = timeRebound;
    }

    public virtual float GetEnergy()
    {
        return currentEnergy;
    }
}
