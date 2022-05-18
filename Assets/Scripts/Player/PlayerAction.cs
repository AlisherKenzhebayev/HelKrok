using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    // TODO: test out with teleport
    // Move the init of collection to Start, poll from playerInventory

    [SerializeField]
    private List<BaseAbilityItemObject> actions;

    private BaseAbilityItemObject currentAction;
    private int current = 0;

    void Start()
    {
        currentAction = actions[current];
    }

    private void Update()
    {
        currentAction = actions[current];
    }

    public void SwitchNextAction() {
        current = (current + 1) % actions.Count;
    }

    public void SwitchPrevAction()
    {
        current = (current - 1) % actions.Count;
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
