using UnityEngine;

public class GravityCustom : MonoBehaviour
{

    [Tooltip("Gravity acceleration")]
    [SerializeField]
    private float gravityAcceleration = 10f;

    [Tooltip("Gravity curve")]
    [SerializeField]
    private AnimationCurve gravityCurve = null;

    private Rigidbody rbReference;
    private float frac = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rbReference = this.GetComponent<Rigidbody>();
        if (rbReference == null) {
            Debug.LogError("Error - no RigidBody component");
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void UpdateFrac(float _frac) {
        frac = _frac;
    }

    private void ApplyGravity()
    {
        float curveMod = gravityCurve.Evaluate(frac);
        rbReference.AddForce(-Vector3.up * rbReference.mass * gravityAcceleration * curveMod, ForceMode.Force);
    }
}
