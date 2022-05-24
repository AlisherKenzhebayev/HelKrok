using System.Collections.Generic;
using UnityEngine;

public class PlayerWon : MonoBehaviour
{
    [SerializeField]
    GameObject winScreen;

    private void Start()
    {
        winScreen.SetActive(false);
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
        GameplayManager.ShowWinScreen();
    }
}
