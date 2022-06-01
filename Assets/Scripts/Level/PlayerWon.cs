using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWon : MonoBehaviour
{
    [SerializeField]
    private GameObject winScreen;

    [SerializeField]
    private bool showByDefault;

    private void Start()
    {
        if(winScreen != null) { 
            winScreen.SetActive(showByDefault);
        }

        GameplayManager.ShowUiScreen(showByDefault);
    }

    private void OnEnable()
    {
        EventManager.StartListening("playerLevelExitDoor", OnPlayerExit);
    }

    private void OnDisable()
    {
        EventManager.StopListening("playerLevelExitDoor", OnPlayerExit);
    }

    private void OnPlayerExit(Dictionary<string, object> obj)
    {
        winScreen.SetActive(true);
        GameplayManager.ShowUiScreen();
    }
}
