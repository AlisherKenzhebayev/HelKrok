using UnityEngine;

public class GrappleInteractableMoveable : GrappleInteractable
{
    [Tooltip("Force applied on the end, towards the grapple")]
    [SerializeField]
    private float ForceAmount = 100f;

    private Rigidbody rb;

    internal override void Start()
    {
        base.Start();

        rb = this.GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("Error - no RigidBody component");
        }
    }

    internal override void Update()
    {
        if (isGrappled)
        {
            Vector3 directionToGrapple = Vector3.Normalize(player.transform.position - this.transform.position);
            rb.AddForce(ForceAmount * directionToGrapple, ForceMode.Force);
        }

        base.Update();
    }
}
