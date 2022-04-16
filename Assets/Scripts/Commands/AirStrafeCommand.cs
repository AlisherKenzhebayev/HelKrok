using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrafeCommand : BaseCommand
{
    private float airStrafeForce = 100f;
    private float maxAirSpeed = 5f;
    private AnimationCurve airForceEffectCurve;

    private Vector3 worldspaceMoveInput;

    public AirStrafeCommand(Rigidbody rb, Vector3 worldspaceMoveInput) : base(rb)
    {
        rbReference = rb;
        this.worldspaceMoveInput = worldspaceMoveInput;
    }

    /// <summary>
    /// Air strafing defined
    /// </summary>
    public override void execute()
    {
        Vector3 velocityNoUp = rbReference.velocity;
        velocityNoUp.y = 0;

        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(velocityNoUp, worldspaceMoveInput);

        // check if the movevector is moving towards or away from the projected velocity
        float dot = Vector3.Dot(worldspaceMoveInput.normalized, rbReference.velocity.normalized);
        bool isAway = Vector3.Dot(worldspaceMoveInput, projVel) <= 0f;

        Debug.Log(projVel.magnitude + " " + isAway);

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < maxAirSpeed || isAway)
        {
            float curveMod = airForceEffectCurve.Evaluate(precisionFloat(projVel.magnitude / maxAirSpeed));
            //precisionFloat(projVel.magnitude / maxAirSpeed)

            // calculate the ideal movement force
            Vector3 vc = worldspaceMoveInput.normalized * airStrafeForce * curveMod;

            // Apply the force
            rbReference.AddForce(vc, ForceMode.Impulse);
        }
    }

    public AirStrafeCommand SetAirStrafeForce(float airStrafeForce)
    {
        this.airStrafeForce = airStrafeForce;
        return this;
    }

    public AirStrafeCommand SetMaxAirSpeed(float maxAirSpeed)
    {
        this.maxAirSpeed = maxAirSpeed;
        return this;
    }

    public AirStrafeCommand SetAirForceEffectCurve(AnimationCurve airForceEffectCurve)
    {
        this.airForceEffectCurve = airForceEffectCurve;
        return this;
    }

    private float precisionFloat(float fValue)
    {
        return Mathf.Round(fValue * 1000f) / 1000f;
    }
}
