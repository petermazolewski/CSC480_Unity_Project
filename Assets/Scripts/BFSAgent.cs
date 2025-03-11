using UnityEngine;

public class BFSAgent : Agent
{
    protected override void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
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

    override protected void StartAI()
    {
        isAI = true;
        currentRoom = GetCurrentRoom();
        doorRoom = FindFirstObjectByType<GenerateMaze>().GetDoorRoom();

        AgentIsMoving = true;

        if (AgentIsMoving && !timerRunning)
        {
            timerRunning = true; // Start the timer when agent starts moving
        }

        path = Pathfinding.FindPathBFS(currentRoom, doorRoom, rooms, keyObjects, 10, 10);
        pathIndex = 0;
    }

    override public void ResetAgent()
    {
        path = null;
        pathIndex = 0;
        isAI = false;
        AgentIsMoving = false;

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(0f, 7f, 0f);

        // Reset timer
        timeElapsed = 0f;
        timerRunning = false;
        timerText.text = "A1 Time: 0.00"; // Reset timer UI
    }
}
