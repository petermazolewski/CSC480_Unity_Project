using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int keys = 0;
    public float speed = 20.0f;

    public int requiredKeys = 3; // number of keys required to open the door

    public Text keyAmount;
    public Text youWin;
    public Text timerText; // Reference to the Text UI element for the timer

    private float timeElapsed = 0f;
    private bool timerRunning = false;
    private bool playerIsMoving = false;
    private bool playerReachedExit = false;

    private GenerateMaze generateMaze;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the timer UI text
        timerText.text = "P1 Time: 0.00";
        keyAmount.text = "Keys: 0"; // Set initial key count to 0
        generateMaze = FindAnyObjectByType<GenerateMaze>(); // Find the GenerateMaze instance
    }

    // Update is called once per frame
    void Update()
    {
        // Detect player movement using arrow keys
        playerIsMoving = false;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            playerIsMoving = true;
        }

        if (playerIsMoving && !timerRunning)
        {
            timerRunning = true; // Start the timer when player starts moving
        }

        // If the timer is running, increment the time
        if (timerRunning && !playerReachedExit)
        {
            timeElapsed += Time.deltaTime;
            timerText.text = "P1 Time: " + timeElapsed.ToString("F2") + " seconds"; // Update the timer UI
        }

        if (Agent.keysCollected && GetCurrentRoom() == generateMaze.GetDoorRoom())
        {
            timerRunning = false;
            playerReachedExit = true;
            Debug.Log("Player reached the doorway. Timer stopped.");
        }

        // Handle player movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, -speed * Time.deltaTime, 0);
        }
    }

    private Room GetCurrentRoom()
    {
        Room[,] rooms = generateMaze.GetRooms();
        foreach (Room room in rooms)
        {
            if (Vector3.Distance(transform.position, room.transform.position) < 0.5f)
                return room;
        }
        return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Key collection
        if (collision.gameObject.tag == "Keys")
        {
            Debug.Log("Key HIT!!!");
            keys++;
            keyAmount.text = "Key: " + keys;
            Destroy(collision.gameObject);
        }

        // Door interaction (exit)
        if (collision.gameObject.tag == "Door")
        {
            timerRunning = false;
            if (keys >= requiredKeys)
            {
                Destroy(collision.gameObject);
                // Stop the timer when the player reaches the exit
                playerReachedExit = true;
                Debug.Log("YOU WIN!!! Final Time: " + timeElapsed.ToString("F2") + " seconds");
            }
            else
            {
                Debug.Log("Collect more keys to open the door!");
            }
        }

        // Collision with enemies (restart the level)
        if (collision.gameObject.tag == "Enemies")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Collision with walls (prevent player from passing through)
        if (collision.gameObject.tag == "Walls")
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
        }
    }

    // Reset the player position and keys upon level reset
    public void ResetPlayer()
    {
        keys = 0;
        transform.position = new Vector3(0f, 0f, 0f);
        timeElapsed = 0f; // Reset the timer
        timerRunning = false;
        timerText.text = "P1 Time: 0.00"; // Reset timer UI
        keyAmount.text = "Key: 0"; // Set initial key count to 0
        playerReachedExit = false; 
    }
}
