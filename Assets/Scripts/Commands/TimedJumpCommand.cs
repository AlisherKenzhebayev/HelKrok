using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedJumpCommand : BaseCommand
{
    public float jumpForce = 600f;

    public TimedJumpCommand(Rigidbody rb) : base(rb)
    {
        this.rbReference = rb;
    }

    public override void execute(Vector3 worldspaceMoveInput)
    {
        rbReference.AddForce(rbReference.transform.up * jumpForce, ForceMode.Impulse);
    }

    public TimedJumpCommand SetJumpForce(float jumpForce) {
        this.jumpForce = jumpForce;
        return this;
    }
}
