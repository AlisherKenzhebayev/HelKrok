public interface IEnergyDepleter
{
    public float GetEnergy();
   
    public float GetPercentEnergy();

    public bool Use(float energyAmount, float percentagePrice);

    public void RestoreFlat(float energyAmount);

    /// <summary>
    /// Changes the passive regeneration speed
    /// </summary>
    public void ChangeRestoreSpeed(float regenSpeed);

    public bool HasEnough(float energyCost);
}
