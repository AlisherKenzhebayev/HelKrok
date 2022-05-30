using System;
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

    private float timerCooldown = float.PositiveInfinity;
    private float currentCooldown;
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
        var curAbility = playerInventory.CurrentAbility();

        if (curAbility == null) {
            return;
        }

        currentAction = (BaseAbilityItemObject)curAbility.item;
        timerCooldown = currentAction.timerCooldown;
    }

    private void FixedUpdate()
    {
        if (isFiring)
        {
            if (currentCooldown <= 0)
            {
                FireAnother();
            }
            else
            {
                FireContinuous();
            }
        }

        UpdateTimers();
    }

    private void FireAnother()
    {
        // Fire another one
        if (!playerEnergyDepleter.Use(currentAction.GetEnergyCost(), 0.0f))
        {
            isFiring = false;
            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
        }

        currentCooldown = timerCooldown;
    }

    private void FireContinuous()
    {
        // Simulate continuous action
        if (!playerEnergyDepleter.Use(currentAction.GetContEnergyCost(), 0.0f))
        {
            // Stop executing if the energy is insufficient
            isFiring = false;
            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
        }

        if (!playerEnergyDepleter.HasEnough(currentAction.GetEnergyCost()))
        {
            // Stop executing if the energy is insufficient
            isFiring = false;
            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
        }
    }

    private void UpdateTimers()
    {
        currentCooldown = Mathf.Max(currentCooldown - Time.fixedDeltaTime, 0);
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

        if (currentAction == null) {
            return;
        }

        isFiring = (bool)obj["amount"];

        if (!isFiring || currentCooldown > 0)
        {
            // Stop firing
            isFiring = false;
            currentAction.Execute(this.gameObject, isFiring, spawnTransform);
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
            currentCooldown = timerCooldown;
        }
    }
}
