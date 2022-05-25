using UnityEngine;

public class AirFrictionCustom : MonoBehaviour
{
    [Tooltip("Simulated sphere radius")]
    [SerializeField]
    private float radius;

    [Tooltip("Air resistance curve")]
    [SerializeField]
    private AnimationCurve airCurve = null;

    private Rigidbody rbReference;
    private float frac = 0f;
    private float fluidDensity = 1.225f;
    private float coefDrag = .47f;

    // Start is called before the first frame update
    void Start()
    {
        rbReference = this.GetComponent<Rigidbody>();
        if (rbReference == null)
        {
            Debug.LogError("Error - no RigidBody component");
        }
    }

    private void FixedUpdate()
    {
        ApplyFriction();
    }

    public void UpdateFrac(float _frac)
    {
        frac = _frac;
    }

    private void ApplyFriction()
    {
        var a = Mathf.PI * radius * radius;
        var v = rbReference.velocity.magnitude;
        var direction = -rbReference.velocity.normalized;
        var forceAmount = (fluidDensity * v * v * coefDrag * a) / 2;
        
        float curveMod = airCurve.Evaluate(frac);
        rbReference.AddForce(direction * forceAmount * curveMod);

        Debug.Log(this.GetType() + " - " + curveMod);
    }
}
