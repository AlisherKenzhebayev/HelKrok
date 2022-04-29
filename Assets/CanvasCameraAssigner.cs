using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraAssigner : MonoBehaviour
{
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = this.GetComponent<Canvas>();
        if (canvas == null) {
            Debug.LogError("Error - no Canvas component");
        }

        canvas.worldCamera = Camera.main;
    }
}
