using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public void InteractStart();
    public void InteractStop();

    /// <summary>
    /// Visualize that this is interactable, one way or another
    /// </summary>
    public void Visualize();
}
