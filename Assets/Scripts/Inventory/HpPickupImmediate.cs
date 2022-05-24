using UnityEngine;

public class HpPickupImmediate : MonoBehaviour
{
    [SerializeField]
    private int amountRestore;
    [SerializeField]
    private string soundEffect;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other has a child component EnergyDepleter
        DamageTaker damageTaker = other.gameObject.GetComponentInChildren<DamageTaker>();
        if (damageTaker == null) {
            Debug.Log("Something without an DamageTaker attempted to pickup hp! " + this.name);
            return;
        }

        damageTaker.RestoreFlat(amountRestore);

        AudioManager.Play(soundEffect);

        //TODO: GameManager.DestroyPickup(this);
        Destroy(this.gameObject);
    }
}
