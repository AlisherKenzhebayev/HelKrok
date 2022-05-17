using System.Collections.Generic;
using UnityEngine;

public class ExitDoorColliderCheck : MonoBehaviour
{
    public LayerMask restrictedLayers;

    private void OnTriggerEnter(Collider other)
    {
        //TODO: add layers
        //if ((restrictedLayers | (1 << collision.gameObject.layer)) == restrictedLayers)
        if (other.CompareTag("exitDoor"))
        {
            EventManager.TriggerEvent("playerLevelExitDoor", new Dictionary<string, object> { { "timeCollision", Time.time } });
        }
    }
}
