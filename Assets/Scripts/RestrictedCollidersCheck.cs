using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is supposed to check for colldiers of specific types 
/// and send out an event to all listeners that out Player 
/// has touched a restricted collider object
/// </summary>
public class RestrictedCollidersCheck : MonoBehaviour
{
    public LayerMask restrictedLayers;

    private void OnCollisionEnter(Collision collision)
    {
        if ((restrictedLayers | (1<<collision.gameObject.layer)) == restrictedLayers)
        {
            EventManager.TriggerEvent("playerCollideRestricted", new Dictionary<string, object> { { "timeCollision", Time.time } });
        }
    }
}
