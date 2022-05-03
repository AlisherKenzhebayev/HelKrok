using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("ExitGame", OnGameExit);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ExitGame", OnGameExit);
    }

    private void OnGameExit(Dictionary<string, object> obj)
    {
        ExitGame();
    }

    public void ExitGame() {
        Application.Quit();
    }
}
