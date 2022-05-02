public class PlayerEnergyDepleter : EnergyDepleter
{
    private float distanceModifier = 1f;

    public override float GetEnergy() {
        return distanceModifier * base.GetEnergy();
    }
}
