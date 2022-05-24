using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCommand : ICommand
{
    internal Rigidbody rbReference;

    public BaseCommand(Rigidbody rb) {
        rbReference = rb;
    }

    public abstract void Execute();
}
