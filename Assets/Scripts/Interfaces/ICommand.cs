using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    /// <summary>
    /// Takes in a worldspace WASD input, tries to execute based of off that.
    /// </summary>
    /// <param name="worldspaceMoveInput"></param>
    public void execute(Vector3 worldspaceMoveInput);
}
