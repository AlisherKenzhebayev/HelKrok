using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDisplayCuffs : MonoBehaviour
{
    [SerializeField]
    private Renderer rendererReference;
    [SerializeField]
    private int arrayN;
    [SerializeField]
    private Material energyShaderMaterial;

    private Material m;

    private void Start()
    {
        m = rendererReference.sharedMaterials[arrayN];
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
