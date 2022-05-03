using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWon : MonoBehaviour
{
    [SerializeField]
    GameObject winScreen;
    // Start is called before the first frame update
    void Start()
    {
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("exitDoor"))
        {
            winScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
