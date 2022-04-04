using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedJumpCommand : BaseCommand
{
    public float jumpForce = 600f;
    private Vector3 worldspaceMoveInput;

    public TimedJumpCommand(Rigidbody rb, Vector3 worldspaceMoveInput) : base(rb)
    {
        this.rbReference = rb;
        this.worldspaceMoveInput = worldspaceMoveInput;
    }

    public override void execute()
    {
        rbReference.AddForce(rbReference.transform.up * jumpForce, ForceMode.Impulse);
    }

    public TimedJumpCommand SetJumpForce(float jumpForce) {
        this.jumpForce = jumpForce;
        return this;
    }
}
