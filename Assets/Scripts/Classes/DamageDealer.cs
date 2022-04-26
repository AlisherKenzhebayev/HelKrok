using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    internal virtual void OnTriggerEnter(Collider other)
    {
        EventManager.TriggerEvent("takeDamage" + other.gameObject.GetInstanceID(), new Dictionary<string, object> { { "amount", 5 } });
        DoDealDamage(other);
    }

    internal virtual void DoDealDamage(Collider other)
    {
        FindObjectOfType<AudioManager>().Play("Hit2");
    }
}