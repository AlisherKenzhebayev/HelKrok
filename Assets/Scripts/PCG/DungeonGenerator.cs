using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    public Vector2 size;
    public int startPos = 0;
    public GameObject room;
    public GameObject key;
    public Vector2 roomOffset; // distance between eachroom
    public Vector2 keyOffset;  // key offset from the middle of the room

    int exitPosition;
    int exitDirection;

    List<int> potentialPositions = new List<int>();
    //int exitPosition;

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateKeyAndExit()
    {
        
        int keyPosition = potentialPositions[UnityEngine.Random.Range(0, potentialPositions.Count - 1)];
        print("key position:" + keyPosition);
        int i = keyPosition % Mathf.FloorToInt(size.x);
        int j = keyPosition / Mathf.FloorToInt(size.x);
        
        var newKey = Instantiate(key, new Vector3(i * roomOffset.x + keyOffset.x, 0, -j * roomOffset.y + keyOffset.y), Quaternion.identity, transform).GetComponent<KeyBehaviour>();
        //newKey.UpdateKey();
        
        exitPosition = potentialPositions[potentialPositions.Count - 1];
        List<int> potentialExitDirection = new List<int>();
        for (int k = 0; k < board[exitPosition].status.Length; k++)
        {
            if (board[exitPosition].status[k] == false)
            {
                potentialExitDirection.Add(k);
            }
        }

        print("exitPosition:" + exitPosition);

        exitDirection = UnityEngine.Random.Range(0, potentialExitDirection.Count);
        


        //board[exitPosition].status[exitDirection] = true;
        print("exitDirection:" + exitDirection);

    }


    RoomBehaviour exitRoom = null;
    RoomBehaviour neighbourOfExitRoom = null;

    int neighbourIncrement(int exitDirection)
    {
        switch (exitDirection)
        {
            case 0:
                return -Mathf.FloorToInt(size.x);
            case 1:
                return Mathf.FloorToInt(size.x);
            case 2:
                return 1;
            case 3:
                return -1;
            default:
                return 0;
        }
        print("something wrong here 1");
        return -2;
        
    }

    int oppositeDirection(int exitDirection)
    {
        switch (exitDirection)
        {
            case 0:
                return 1;
            case 1:
                return 0;
            case 2:
                return 3;
            case 3:
                return 2;
            default:
                print("something wrong here 2");
                return 0;
        }
    }
    void GenerateDungeon()
    {
        
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var newRoom = Instantiate(room, new Vector3(i * roomOffset.x, 0, -j * roomOffset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                newRoom.UpdateRoom(board[Mathf.FloorToInt(i + j * size.x)].status);
                newRoom.name += " " + i + "-" + j;
                //print("x:" + (exitPosition + neighbourIncrement(exitDirection, true)) / Mathf.FloorToInt(size.x));
                //print("y:" + (exitPosition + neighbourIncrement(exitDirection, false)) % Mathf.FloorToInt(size.x));
                //print("x:" + neighbourIncrement(exitDirection));
                //print("y:" + neighbourIncrement(exitDirection));
                if (i == exitPosition % Mathf.FloorToInt(size.x) && j == exitPosition / Mathf.FloorToInt(size.x))
                {
                    exitRoom = newRoom;
                    
                }
                else if (i == (exitPosition + neighbourIncrement(exitDirection)) % Mathf.FloorToInt(size.x) && j == (exitPosition + neighbourIncrement(exitDirection)) / Mathf.FloorToInt(size.x))
                {
                    neighbourOfExitRoom = newRoom;
                }



            }
        }
    }

    void ModifyExitRoom()
    {
        //print("here-1");
        print(exitRoom);
        print(neighbourOfExitRoom);
        
        exitRoom.UpdateExitWalls(exitDirection);
        exitRoom.UpdateExitRoomOutlet(exitDirection);
        Console.WriteLine("hereyes");
        //print("positive:" + exitDirection);
        //print("negative:" + oppositeDirection(exitDirection));
        if (neighbourOfExitRoom != null)
        {
            //print("inside");
            neighbourOfExitRoom.UpdateExitWalls(oppositeDirection(exitDirection));
        }

        /*
        if (!((exitPosition / size.x == 0 && exitDirection == 0) ||
                            (exitPosition % size.x == 0 && exitDirection == 3) ||
                            (exitPosition / size.x == size.y - 1 && exitDirection == 1) ||
                            (exitPosition % size.x == size.x - 1 && exitDirection == 2)))
        {
        }
        */

    }

    void MazeGenerator()
    {
        board = new List<Cell>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int pathLength = 0;
        int longestPathLength = int.MinValue;
        int longestPathIndex = 0;

        int k = 0;
        bool firstDeadEndEncounter = true;
        while (k < 1000)
        {
            k++;
            board[currentCell].visited = true;
            

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);
            if (neighbors.Count == 0)
            {
                //print("potential position:" + currentCell);
                if (firstDeadEndEncounter)
                {
                    potentialPositions.Add(currentCell);
                    firstDeadEndEncounter = false;
                }
                
                if (pathLength > longestPathLength)
                {
                    longestPathLength = pathLength;
                    longestPathIndex = potentialPositions.Count - 1;
                }
                
                if (path.Count == 0)
                {
                    potentialPositions.Add(potentialPositions[longestPathIndex]);
                    potentialPositions.Remove(longestPathIndex);
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                    pathLength -= 1;
                }
            }
            else
            {
                firstDeadEndEncounter = true;
                pathLength += 1;
                
                path.Push(currentCell);
                int newCell = neighbors[UnityEngine.Random.Range(0, neighbors.Count)]; 
                if (newCell > currentCell)
                {
                    // down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    // up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateKeyAndExit();
        GenerateDungeon();
        ModifyExitRoom();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }
        //check down neighbor
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + size.x));
        }
        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }
        //check left neighbor
        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }
        return neighbors;
    }
}
