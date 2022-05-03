using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileSpawner))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> actions;

    private GameObject currentAction;
    private int current = 0;
    private ProjectileSpawner projectileSpawner;

    void Start()
    {
        currentAction = actions[current];
        projectileSpawner = this.GetComponent<ProjectileSpawner>();
        projectileSpawner.ProjectileToSpawn = currentAction;

        projectileSpawner.enabled = false;
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
        projectileSpawner.enabled = (bool)obj["amount"] ? true : false;
    }
}
