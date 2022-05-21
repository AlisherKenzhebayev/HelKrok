using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    // TODO: test out with teleport

    private BaseAbilityItemObject currentAction;
    private int current = 0;

    private GameObject player;
    private Inventory playerInventory;

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
    }

    private void Update()
    {
        currentAction = (BaseAbilityItemObject)playerInventory.CurrentAbility().item;
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

        currentAction.Execute(this.gameObject, (bool)obj["amount"]);
    }
}
