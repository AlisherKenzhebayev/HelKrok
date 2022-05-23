using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    // TODO: test out with teleport
    public Transform spawnTransform;

    private BaseAbilityItemObject currentAction;

    private GameObject player;
    private Inventory playerInventory;
    private EnergyDepleter playerEnergyDepleter;

    private bool isFiring = false;

    void Start()
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

        playerEnergyDepleter = player.GetComponentInChildren<EnergyDepleter>();
        if (playerEnergyDepleter == null)
        {
            Debug.LogError("Error - no EnergyDepleter child component exists");
        }
    }

    private void Update()
    {
        currentAction = (BaseAbilityItemObject)playerInventory.CurrentAbility().item;
    }

    private void FixedUpdate()
    {
        if (isFiring)
        {
            // Simulate continuous action
            if (!playerEnergyDepleter.Use(currentAction.GetContEnergyCost(), 0.0f))
            {
                // Stop executing if the energy is insufficient
                isFiring = false;
                currentAction.Execute(this.gameObject, isFiring, spawnTransform);
            }
        }
    }

    public void SwitchNextAction() {
        playerInventory.ChangeNextAbility();
    }

    public void SwitchPrevAction()
    {
        playerInventory.ChangePrevAbility();
    }

    internal virtual void OnEnable()
    {
        EventManager.StartListening("PlayerActionButton", OnEnableAction);
    }

    internal virtual void OnDisable()
    {
        EventManager.StopListening("PlayerActionButton", OnEnableAction);
    }

    private void OnEnableAction(Dictionary<string, object> obj)
    {
        // TODO: define some common pattern of interaction here

        isFiring = (bool)obj["amount"];

        if (!isFiring)
        {
            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
            return;
        }
        else
        {
            if (playerEnergyDepleter.HasEnough(currentAction.GetEnergyCost()))
            {
                isFiring = playerEnergyDepleter.Use(currentAction.GetEnergyCost(), 0.0f);
            }
            else
            {
                isFiring = false;
            }

            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
            return;
        }
    }
}
