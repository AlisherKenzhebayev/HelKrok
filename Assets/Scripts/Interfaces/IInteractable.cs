using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Execute(bool state);

    /// <summary>
    /// Visualize that this is interactable, one way or another
    /// </summary>
    public void Visualize();
}
