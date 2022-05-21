using UnityEngine;

public class EnergyPickupImmediate : MonoBehaviour
{
    [SerializeField]
    private float amountRestore;
    [SerializeField]
    private string soundEffect;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other has a child component EnergyDepleter
        EnergyDepleter energyDepleter = other.gameObject.GetComponentInChildren<EnergyDepleter>();
        if (energyDepleter == null) {
            Debug.Log("Something without an EnergyDepleter attempted to pickup energy! " + this.name);
            return;
        }

        energyDepleter.RestoreFlat(amountRestore);

        AudioManager.Play(soundEffect);

        //TODO: GameManager.DestroyPickup(this);
        Destroy(this.gameObject);
    }
}
