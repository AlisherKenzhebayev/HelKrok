public class PlayerEnergyDepleter : EnergyDepleter
{

    private float distanceModifier;

    public override float GetEnergy() {
        return distanceModifier* base.GetEnergy();
    }
}
