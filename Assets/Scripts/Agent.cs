using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Agent : MonoBehaviour
{
    public int keys = 0;
    public float speed = 5.0f;
    public Text keyAmount;
    public Text youWin;

    private bool isAI = false;
    private List<Room> path;
    private int pathIndex = 0;
    private Room[,] rooms;
    private Room currentRoom;
    private Room doorRoom;

    void Start()
    {
        rooms = FindObjectOfType<GenerateMaze>().GetRooms();
        currentRoom = GetCurrentRoom();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))  
        {
            isAI = !isAI; // Toggle AI mode on/off
            if (isAI)
            {
                StartAI();  // Start AI pathfinding
            }
        }
        if (!isAI)
        {
            HandleUserInput();
        }
        else if (path != null && pathIndex < path.Count)
        {
            MoveToNextRoom();
        }
    }

    public void StartAI()
    {
        isAI = true;
        currentRoom = GetCurrentRoom();
        doorRoom = FindObjectOfType<GenerateMaze>().GetDoorRoom();
        path = Pathfinding.FindPath(currentRoom, doorRoom, rooms, 10, 10);
        pathIndex = 0;
    }

    private void MoveToNextRoom()
    {
        if (pathIndex >= path.Count)
        {
            isAI = false;
            return;
        }

        Room targetRoom = path[pathIndex];
        Vector3 targetPosition = targetRoom.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            pathIndex++;
        }
    }

    private Room GetCurrentRoom()
    {
        foreach (Room room in rooms)
        {
            if (Vector3.Distance(transform.position, room.transform.position) < 0.5f)
                return room;
        }
        return null;
    }
    private void HandleUserInput()
    {
    if(Input.GetKey(KeyCode.LeftArrow))
    {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
    }
    if(Input.GetKey(KeyCode.RightArrow))
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
    }
    if(Input.GetKey(KeyCode.UpArrow))
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
    }
    if(Input.GetKey(KeyCode.DownArrow))
    {
        transform.Translate(0, -speed * Time.deltaTime, 0);
    }
    }
}
