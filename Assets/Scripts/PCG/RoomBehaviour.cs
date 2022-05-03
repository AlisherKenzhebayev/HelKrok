using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour: MonoBehaviour
{
    public GameObject[] walls; // 0 - Up 1 - Down 2 - Right 3 Left
    public GameObject[] doors;
    public GameObject[] closedExits;
    public GameObject[] openedExits;
    public int exitDirection;

    /*
    public bool[] testStatus;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRoom(testStatus);
    }
    */

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
            closedExits[i].SetActive(false);
            openedExits[i].SetActive(false);
        }
    }
    public void UpdateExitWalls(int exitDirection)
    {
        //print("here0");
        walls[exitDirection].SetActive(false);
    }

    public void UpdateExitRoomOutlet(int exitDirection)
    {
        closedExits[exitDirection].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
