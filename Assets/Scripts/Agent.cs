using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Agent : MonoBehaviour
{
    public int keys = 0;
    public float speed = 20.0f;

    public int requiredKeys = 3; //number of keys required to open the door

    public Text keyAmount;
    public Text youWin;
    public Text timerText;

    protected bool bfs = false;
    public bool isAI = false;
    protected List<Room> path;
    protected int pathIndex = 0;
    protected Room[,] rooms;
    protected Room currentRoom;
    protected Room doorRoom;
    protected List<GameObject> keyObjects;

    protected bool moveToDoor = false;
    protected bool exiting = false;

    // Timer variables
    protected float moveRightTimer = 0f;
    protected float moveRightDuration = 1f; // Duration to move to the right (in seconds)

    protected float timeElapsed = 0f;
    protected bool timerRunning = false;
    
    protected virtual void Start()
    {
        GameObject bfsAgent = GameObject.FindWithTag("BFSAgent");
        Physics2D.IgnoreCollision(bfsAgent.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());

        GameObject aStarAgent = GameObject.FindWithTag("AStarAgent");
        Physics2D.IgnoreCollision(aStarAgent.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());

        GameObject player = GameObject.FindWithTag("Player");
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());

        rooms = FindFirstObjectByType<GenerateMaze>().GetRooms();
        keyObjects = FindFirstObjectByType<GenerateMaze>().GetSpawnedKeys();
        currentRoom = GetCurrentRoom();
    }

    protected virtual void Update()
    {   
        if (AllKeysCollected())
        {
            exiting = true;
        }
        // Check if the current room is the door room
        if (GetCurrentRoom() == doorRoom && !moveToDoor && exiting)
        {
            timerRunning = false;
            Debug.Log("in door room");
            if (keys >= requiredKeys)
            {
                moveToDoor = true;
                moveRightTimer = 0f; // Reset the timer
            }
            
        }
        if (moveToDoor)
        {
            if (moveRightTimer < moveRightDuration)
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
                moveRightTimer += Time.deltaTime;
                Debug.Log("moving");
            }
            else
            {
                moveToDoor = false; // Stop moving after the duration
            }
        }
                
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isAI)
            {
                StartAI();
            }
        }

        if (path != null && pathIndex < path.Count)
        {
            MoveToNextRoom();
        }
    }

    protected virtual void StartAI()
    {

    }

    public virtual void ResetAgent()
    {
    }

    protected void MoveToNextRoom()
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

    protected Room GetCurrentRoom()
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

    private bool AllKeysCollected()
    {
        // Check if all keys have been collected by any agent
        int totalKeysCollected = 0;
        Agent[] agents = FindObjectsOfType<Agent>();
        foreach (Agent agent in agents)
        {
            totalKeysCollected += agent.keys;
        }
        Player[] player = FindObjectsOfType<Player>();
        totalKeysCollected += player[0].keys;
        return totalKeysCollected >= keyObjects.Count;
    }
}

