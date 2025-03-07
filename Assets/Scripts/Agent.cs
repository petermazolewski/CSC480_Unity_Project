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

    private bool bfs = false;
    public bool isAI = false;
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            bfs = !bfs;
        }

        if (Input.GetKeyDown(KeyCode.Q))  
        {
            // isAI = !isAI; // Toggle AI mode on/off
            if (isAI)
            {
                StartAI();  // Start AI pathfinding
            }
        }

        if (!isAI)
        {
            // HandleUserInput();
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

        if (!bfs)
        {
            path = Pathfinding.FindPath(currentRoom, doorRoom, rooms, 10, 10);
        }
        else
        {
            path = Pathfinding.FindPathBFS(currentRoom, doorRoom, rooms, 10, 10);
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

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(0f, 0f, 0f);
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Keys")
        {
            Debug.Log("Key HIT!!!");
            keys++;
            // keyAmount.text = "Keys: " + keys;
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Door") // silver
        {
            // Destroy(collision.gameObject);
            // youWin.text = "YOU WIN!!!";
            if (keys >= requiredKeys)
            {
                Destroy(collision.gameObject);
                // youWin.text = "YOU WIN!!!";
                Debug.Log("YOU WIN!!!");
            }
            else
            {
                Debug.Log("Collect more keys to open the door!");
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
