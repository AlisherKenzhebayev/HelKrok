using System.Collections.Generic;
using UnityEngine;

public class PlayerLost : MonoBehaviour
{
    [SerializeField]
    private GameObject debugText;

    private void OnEnable()
    {
        EventManager.StartListening("playerCollideRestricted", OnColliderRestricted);
    }

    private void OnDisable()
    {
        EventManager.StopListening("playerCollideRestricted", OnColliderRestricted);
    }

    private void OnColliderRestricted(Dictionary<string, object> obj)
    {
        AudioManager.Play("RestrictedCollision");
        debugText.SetActive(true);
    }
}
