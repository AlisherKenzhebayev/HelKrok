using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject openedExit;
    public GameObject closedExit;

    /*
    private void OnTriggerStay(Collider other)
    {
        closedExit.transform.parent.gameObject.SetActive(false);
        openedExit.transform.parent.gameObject.SetActive(true);
        //closedExit.SetActive(false);
        //openedExit.SetActive(true);
    }
    */


    // Start is called before the first frame update
    void Start()
    {
        //openedExit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
