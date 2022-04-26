using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLost : MonoBehaviour
{
    [SerializeField]
    private GameObject debugText;

    // Player components
    private GameObject spawn;
    private GameObject player;
    private PlayerController playerController;
    private Rigidbody playerRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        if (spawn == null)
        {
            Debug.LogError("Error - no Spawn tag exists");
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Error - no PlayerController component exists");
        }

        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerController == null)
        {
            Debug.LogError("Error - no RigidBody component exists");
        }

        ResetPlacement();
    }

    private void ResetPlacement()
    {
        playerRigidbody.velocity = Vector3.zero;
        player.transform.position = spawn.transform.position;
        player.transform.rotation = spawn.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlacement();
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("playerCollideRestricted", OnColliderRestricted);
    }

    private void OnColliderRestricted(Dictionary<string, object> obj)
    {
        FindObjectOfType<AudioManager>().Play("RestrictedCollision");
        debugText.SetActive(true);
    }
}
