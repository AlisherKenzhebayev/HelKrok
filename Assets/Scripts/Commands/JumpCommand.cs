using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCommand : BaseCommand
{
    [Tooltip("Jump force")]
    public float jumpForce = 300f;

    public JumpCommand(Rigidbody rb) : base(rb)
    {
        this.rbReference = rb;
    }

    public override void execute(Vector3 worldspaceMoveInput)
    {
        rbReference.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public JumpCommand SetJumpForce(float jumpForce) {
        this.jumpForce = jumpForce;
        return this;
    }
}
