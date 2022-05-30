using UnityEngine;

public class EnergyScalerSphere : MonoBehaviour
{
    private Vector3 localScale;

    private GameObject player;
    private EnergyDepleter playerEnergyDepleter;

    // Start is called before the first frame update
    void Start()
    {
        localScale = this.transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerEnergyDepleter = player.GetComponentInChildren<EnergyDepleter>();
        if (playerEnergyDepleter == null)
        {
            Debug.LogError("Error - no EnergyDepleter child component exists");
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = 2 * new Vector3(
            playerEnergyDepleter.currentEnergy, 
            playerEnergyDepleter.currentEnergy, 
            playerEnergyDepleter.currentEnergy);
    }
}
