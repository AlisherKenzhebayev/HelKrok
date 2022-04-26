using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public void Execute();

    /// <summary>
    /// Visualize that this is interactable, one way or another
    /// </summary>
    public void Visualize();
}
