using UnityEngine;

public class RotateHand : MonoBehaviour
{
    private GameObject player;
    private EnergyDepleter playerEnergyDepleter;

    [Tooltip("Camera follow transform")]
    [SerializeField]
    private Transform playerCamera = null;

    [SerializeField]
    private GrappleVisualizer visualizer = null;

    // Start is called before the first frame update
    void Start()
    {
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

        if (playerCamera == null)
        {
            Debug.LogError(playerCamera.GetType() + " not specified");
        }

        if (visualizer == null)
        {
            Debug.LogError(visualizer.GetType() + " not specified");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogWarning("ASD - " + visualizer.DirectionToGrapple.normalized);
        Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(playerCamera.forward.normalized + visualizer.DirectionToGrapple.normalized) );
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, 0.03f);
    }
}
