using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuObject : MonoBehaviour
{
    [SerializeField]
    private Canvas pauseMenuCanvas;

    [SerializeField]
    private bool defaultVisibility = false;
    private bool isVisible;

    private void Start()
    {
        isVisible = defaultVisibility;
        pauseMenuCanvas.enabled = isVisible;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetPauseMenuKeyDown()) {
            if (isVisible) {
                GameplayManager.ContinueGameTime();
                pauseMenuCanvas.enabled = false;
            }
            else {
                GameplayManager.PauseGameTime();
                pauseMenuCanvas.enabled = true;
            }
        }

        isVisible = pauseMenuCanvas.enabled;
    }
}
