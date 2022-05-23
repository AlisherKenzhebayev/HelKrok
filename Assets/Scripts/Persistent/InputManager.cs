using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager inputManager;

    public static InputManager instance
    {
        get {
            inputManager = FindObjectOfType<InputManager>();
            if (!inputManager)
            {
                Debug.LogError("There needs to be one active InputManager script on a GameObject in your scene.");
            }
            else
            {
                inputManager.Init();

                DontDestroyOnLoad(inputManager);
            }

            return inputManager;
        }
    }

    void Init()
    {
        return;
    }

    public static float getMouseHorizontal()
    {
        return Input.GetAxis(GameConstants.k_MouseAxisNameHorizontal);
    }

    public static float getMouseVertical()
    {
        return Input.GetAxis(GameConstants.k_MouseAxisNameVertical);
    }

    public static float getKeyHorizontal()
    {
        return Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal);
    }

    public static float getKeyVertical()
    {
        return Input.GetAxisRaw(GameConstants.k_AxisNameVertical);
    }

    public static bool GetJumpButtonDown() {
        return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
    }

    public static bool GetActionButtonDown()
    {
        return Input.GetButtonDown(GameConstants.k_ButtonNameAction);
    }

    public static bool GetActionButtonUp()
    {
        return Input.GetButtonUp(GameConstants.k_ButtonNameAction);
    }

    internal static bool GetResetKeyDown()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    internal static bool GetRestartKeyDown()
    {
        return Input.GetKeyDown(KeyCode.T);
    }

    public static Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(getKeyHorizontal(), 0f, getKeyVertical());

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }

    internal static bool GetInventoryKeyDown()
    {
        return Input.GetKeyDown(KeyCode.I);
    }
}
