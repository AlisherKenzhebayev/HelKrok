using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField]
    private GameObject ClosedDoorPrefab;
    [SerializeField]
    private GameObject OpenedDoorPrefab;

    private GameObject player;
    private Inventory playerInventory;
    private GameObject closedDoor;
    private GameObject openDoor;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerInventory = player.GetComponentInChildren<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Error - no playerInventory child component exists");
        }

        closedDoor = Instantiate(ClosedDoorPrefab, this.transform);
        openDoor = Instantiate(OpenedDoorPrefab, this.transform);
    }

    private void Update()
    {
        if (playerInventory.FindItemName("key"))
        {
            closedDoor.SetActive(false);
            openDoor.SetActive(true);
        }
        else {
            closedDoor.SetActive(true);
            openDoor.SetActive(false);
        }

    }
}
