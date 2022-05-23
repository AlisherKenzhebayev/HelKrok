using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : BaseCommand
{
    private float backwardsSpeedCoef = 0.15f;
    private float groundSpeed = 10f;

    private Vector3 worldspaceMoveInput;

    public MoveCommand(Rigidbody rb, Vector3 worldspaceMoveInput) : base(rb)
    {
        this.rbReference = rb;
        this.worldspaceMoveInput = worldspaceMoveInput;
    }

    public override void Execute()
    {
        float dot = Vector3.Dot(rbReference.transform.forward, worldspaceMoveInput);
        float speedMod = 0f;

        if (dot < 0)
        {
            speedMod = Mathf.SmoothStep(0, 1, -dot) * backwardsSpeedCoef;
        }

        Vector3 characterVelocity = worldspaceMoveInput * groundSpeed * (1 - speedMod);
        characterVelocity.y = rbReference.velocity.y;

        rbReference.velocity = characterVelocity;
    }

    public MoveCommand SetGroundSpeed(float groundSpeed) {
        this.groundSpeed = groundSpeed;
        return this;
    }

    public MoveCommand SetBackwardsSpeedCoef(float backwardsSpeedCoef)
    {
        this.backwardsSpeedCoef = backwardsSpeedCoef;
        return this;
    }
}
