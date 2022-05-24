using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [SerializeField]
    private BaseItemObject itemObject;
    [SerializeField]
    private string soundEffect;

    private void Start()
    {
        if (itemObject == null) {
            Debug.LogError("itemObject is null! " + this.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other has a child component Inventory
        Inventory inventory = other.gameObject.GetComponentInChildren<Inventory>();
        if (inventory == null) {
            Debug.Log("Something without an inventory attempted to pickup an item! " + this.name);
            return;
        }

        inventory.AddItem(itemObject, 1);

        AudioManager.Play(soundEffect);

        //TODO: GameManager.DestroyPickup(this);
        Destroy(this.gameObject);
    }
}
