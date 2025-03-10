using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Agent : MonoBehaviour
{
    public int keys = 0;
    public float speed = 20.0f;
    // public GameObject door;

    public int requiredKeys = 3; //number of keys required to open the door

    public Text keyAmount;
    public Text youWin;

    public Text timerText;

    private bool bfs = false;
    public bool isAI = false;
    private List<Room> path;
    private int pathIndex = 0;
    private Room[,] rooms;
    private Room currentRoom;
    private Room doorRoom;
    private List<GameObject> keyObjects;

    private bool moveToDoor = false;

    // Timer variables
    private float moveRightTimer = 0f;
    private float moveRightDuration = 2f; // Duration to move to the right (in seconds)

    private float timeElapsed = 0f;
    private bool timerRunning = false;
    private bool AgentIsMoving = false;
    void Start()
    {
        rooms = FindObjectOfType<GenerateMaze>().GetRooms();
        keyObjects = FindObjectOfType<GenerateMaze>().GetSpawnedKeys();
        currentRoom = GetCurrentRoom();
        timerText.text = "A1 Time: 0.00";
    }

    void Update()
    {   
    // Check if the agent should move to the door
        if (moveToDoor)
        {
            if (moveRightTimer < moveRightDuration)
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
                moveRightTimer += Time.deltaTime;
            }
            else
            {
                moveToDoor = false; // Stop moving after the duration
            }
        }

        // Check if the current room is the door room
        if (GetCurrentRoom() == doorRoom && !moveToDoor)
        {
            moveToDoor = true;
            moveRightTimer = 0f; // Reset the timer
            Debug.Log("in door room");
        }
                
        // If the timer is running and the agent hasn't reached the exit, update the time
        if (timerRunning)
        {
            timeElapsed += Time.deltaTime;
            timerText.text = "A1 Time: " + timeElapsed.ToString("F2") + " seconds"; // Update the timer UI
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            bfs = !bfs;
        }

        if (Input.GetKeyDown(KeyCode.Q))  
        {
            if (isAI)
            {
                StartAI();  // Start AI pathfinding
            }
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
        doorRoom = FindAnyObjectByType<GenerateMaze>().GetDoorRoom();

        AgentIsMoving = true;

        if (AgentIsMoving && !timerRunning)
        {
            timerRunning = true; // Start the timer when agent starts moving
        }

        if (!bfs)
        {
            path = Pathfinding.FindPathAStar(currentRoom, doorRoom, rooms, keyObjects, 10, 10);
        }
        else
        {
            path = Pathfinding.FindPathBFS(currentRoom, doorRoom, rooms, keyObjects, 10, 10);
            Debug.Log("using bfs");
        }
        pathIndex = 0;
    }

    public void ResetAgent()
    {
        // Reset pathfinding state
        path = null;
        pathIndex = 0;
        isAI = false;
        AgentIsMoving = false;

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(0f, 0f, 0f);

        // Reset timer
        timeElapsed = 0f;
        timerRunning = false;
        timerText.text = "A1 Time: 0.00"; // Reset timer UI
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Keys")
        {
            Debug.Log("Key HIT!!!");
            keys++;
            // keyAmount.text = "Keys: " + keys;
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Door")
        {
            if (keys >= requiredKeys)
            {
                Destroy(collision.gameObject);
                timerRunning = false;
                Debug.Log("YOU WIN!!! Final Time: " + timeElapsed.ToString("F2") + " seconds");
            }

        }

        if(collision.gameObject.tag == "Enemies")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(collision.gameObject.tag == "Walls")
        {
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
                Debug.Log("Left");
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
                Debug.Log("Right");
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(0, -speed * Time.deltaTime, 0);
                Debug.Log("Up");
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
                Debug.Log("Down");
            }
        }
    }
}

