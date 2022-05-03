using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    internal int damage = 5;
    [SerializeField]
    internal string soundToPlay = "Hit2";

    internal virtual void OnTriggerEnter(Collider other)
    {
        EventManager.TriggerEvent("takeDamage" + other.gameObject.GetInstanceID(), new Dictionary<string, object> { { "amount", damage } });
        DoDealDamage(other);
    }

    internal virtual void DoDealDamage(Collider other)
    {
        AudioManager.Play(soundToPlay);
    }
}