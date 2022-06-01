using UnityEngine;

public class GrappleMoveCommand : BaseCommand
{
    private float moveForce = 100f;
    private AnimationCurve forceEffectCurve;

    private Vector3 worldspaceMoveInput;
    private Vector3 lookVector;

    public GrappleMoveCommand(Rigidbody rb, Vector3 worldMoveInput) : base(rb)
    {
        rbReference = rb;
        this.worldspaceMoveInput = worldMoveInput;
    }

    public static Vector3 FilterYZ(Vector3 _vector)
    {
        _vector.y = 0;
        _vector.z = 0;
        return _vector;
    }

    public override void Execute()
    {
        Vector3 velocityNoUp = rbReference.velocity;
        velocityNoUp.y = 0;

        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(velocityNoUp, worldspaceMoveInput);

        //Debug.Log(this.GetType() + " - " + projVel.magnitude + " - Velocity: " + rbReference.velocity);
        
        {
            // When max disaligned -> bigger result
            float curveMod = forceEffectCurve.Evaluate(1 - Vector3.Dot(lookVector.normalized, rbReference.velocity.normalized));

            //Debug.Log(this.GetType() + " - " + lookVector.normalized + " " + rbReference.velocity.normalized);

            Vector3 vc = worldspaceMoveInput.normalized * moveForce 
                //* precisionFloat(projVel.magnitude / rbReference.velocity.magnitude) 
                * curveMod;

            //Debug.Log(this.GetType() + " - CurveMod: " + curveMod + " - Magnitude: " + vc.magnitude + " - Input: " + worldspaceMoveInput);
            
            rbReference.AddForce(vc, ForceMode.Force);
        }
    }

    public GrappleMoveCommand SetMoveForce(float _force)
    {
        this.moveForce = _force;
        return this;
    }

    public GrappleMoveCommand SetLookVector(Vector3 _look)
    {
        this.lookVector = _look;
        return this;
    }

    public GrappleMoveCommand SetForceEffectCurve(AnimationCurve _forceCurve)
    {
        this.forceEffectCurve = _forceCurve;
        return this;
    }

    private float precisionFloat(float fValue)
    {
        return Mathf.Round(fValue * 1000f) / 1000f;
    }
}
