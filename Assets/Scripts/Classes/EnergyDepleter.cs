using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For now only has current and maxEnergy
/// </summary>
public abstract class EnergyDepleter : MonoBehaviour, IEnergyDepleter
{
    [SerializeField]
    [Tooltip("Same as maxEnergy, works with that assumption")]
    internal float currentEnergy = 100;
    [SerializeField]
    [Tooltip("minEnergy, undepletable number")]
    internal float minEnergy = 10;
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
            currentEnergy = Mathf.Clamp(currentEnergy + currentRestoreSpeed, minEnergy, maxEnergy);
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

    public virtual void Start()
    {
        maxEnergy = currentEnergy;
    }

    public virtual void Update()
    {
        EventManager.TriggerEvent("currentEnergyPlayer", new Dictionary<string, object> { { "amount", this.GetPercentEnergy() } });
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

    public virtual bool HasEnough(float energyCost = 25f)
    {
        if (currentEnergy >= energyCost) {
            return true;
        }

        return false;
    }

    public void RestoreFlat(float energyAmount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + energyAmount, minEnergy, maxEnergy);
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
        currentEnergy = Mathf.Clamp(currentEnergy * (1 - percentagePrice), minEnergy, maxEnergy);
        currentTimeRebound = timeRebound;
    }

    public virtual float GetEnergy()
    {
        return currentEnergy;
    }

    public virtual float GetPercentEnergy()
    {
        return Mathf.Clamp(currentEnergy / maxEnergy, 0f, 1f);
    }
}
