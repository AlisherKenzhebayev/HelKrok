using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCommand : BaseCommand
{
    private float jumpForce = 300f;

    private Vector3 worldspaceMoveInput;

    public JumpCommand(Rigidbody rb, Vector3 worldspaceMoveInput) : base(rb)
    {
        this.rbReference = rb;
        this.worldspaceMoveInput = worldspaceMoveInput;
    }

    public override void execute()
    {
        rbReference.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public JumpCommand SetJumpForce(float jumpForce) {
        this.jumpForce = jumpForce;
        return this;
    }
}
