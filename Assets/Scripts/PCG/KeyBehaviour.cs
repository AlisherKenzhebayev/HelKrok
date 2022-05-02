using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehaviour : MonoBehaviour
{
    //public GameObject key;
    //public GameObject doorCollider;

    public GameObject openedExit;
    public GameObject closedExit;

    public void UpdateKey()
    {
        //key.SetActive(true);
    }


    private void OnTriggerStay(Collider other)
    {
        //doorCollider.GetComponent<BoxCollider>().enabled = true;
        gameObject.SetActive(false);
        //closedExit.transform.parent.gameObject.SetActive(false);
        //openedExit.transform.parent.gameObject.SetActive(true);
        //GameObject exitRoom = GameObject.FindGameObjectWithTag("www");
        RoomBehaviour[] exitRooms = GameObject.FindObjectsOfType<RoomBehaviour>();

        //print("here4: " + exitRoom.transform.parent.gameObject);
        //print("here5: " + exitRoom);
 
        //RoomBehaviour exitRoomVar = exitRoom.GetComponent<RoomBehaviour>();
        for (int i = 0; i < exitRooms.Length; i++)
        {
            //print("here6: " + exitRoom[i]);
            /*
            if (exitRoom.transform.GetChild(i) != null)
            {
                exitRoom.transform.GetChild(i).gameObject.SetActive(false);
            }
            */
            for (int j = 0; j < exitRooms[i].transform.childCount; j++)
            {
                for (int k = 0; k < exitRooms[i].transform.GetChild(j).childCount; k++)
                {
                    if (exitRooms[i].transform.GetChild(j).name == "ClosedExitWalls") {
                        if (exitRooms[i].transform.GetChild(j).GetChild(k).gameObject.activeSelf)
                        {
                            exitRooms[i].transform.GetChild(j).GetChild(k).gameObject.SetActive(false);
                            exitRooms[i].transform.GetChild(j + 1).GetChild(k).gameObject.SetActive(true);
                            //print(exitRooms[i].transform.GetChild(j).GetChild(k).name);
                            break;
                        }
                    } 
                    {
                        
                    }
                }
                
            }
        }
        //exitRoomVar.closedExits[exitRoomVar.exitDirection].SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
