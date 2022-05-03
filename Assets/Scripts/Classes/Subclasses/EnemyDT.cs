using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy the specified GO.
/// </summary>
public class EnemyDT : DamageTaker
{
    [SerializeField]
    private GameObject gameObjectToDisable;

    internal override void DoDeath()
    {
        Destroy(gameObjectToDisable);
        return;
    }
}