using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public InputManager() { 
    
    }

    public float getMouseHorizontal()
    {
        return Input.GetAxis(GameConstants.k_MouseAxisNameHorizontal);
    }

    public float getMouseVertical()
    {
        return Input.GetAxis(GameConstants.k_MouseAxisNameVertical);
    }

    public float getKeyHorizontal()
    {
        return Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal);
    }

    public float getKeyVertical()
    {
        return Input.GetAxisRaw(GameConstants.k_AxisNameVertical);
    }

    public bool GetJumping() {
        return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
    }

    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(getKeyHorizontal(), 0f, getKeyVertical());

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }
}
