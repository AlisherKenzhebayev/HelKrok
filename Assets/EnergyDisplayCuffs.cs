using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDisplayCuffs : MonoBehaviour
{
    [SerializeField]
    private Material energyShaderMaterial;

    private Material m;

    private void Start()
    {
        m = energyShaderMaterial;
    }

    internal virtual void OnEnable()
    {
        EventManager.StartListening("currentEnergyPlayer", OnEnergyChange);
    }

    internal virtual void OnDisable()
    {
        EventManager.StopListening("currentEnergyPlayer", OnEnergyChange);
    }

    private void OnEnergyChange(Dictionary<string, object> obj)
    {
        m.SetFloat("Energy_", (float)obj["amount"]);
    }
}
