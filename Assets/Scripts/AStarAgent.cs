using UnityEngine;

public class AStarAgent : Agent
{
    protected override void Start()
    {
        base.Start();
        timerText.text = "A* Time: 0.00";
    }

    protected override void Update()
    {
        base.Update();
        if (timerRunning) {
            timeElapsed += Time.deltaTime;
            timerText.text = "A* Time: " + timeElapsed.ToString("F2") + " seconds"; // Update the timer UI
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

        path = Pathfinding.FindPathAStar(currentRoom, doorRoom, rooms, keyObjects, 10, 10);
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
        transform.position = new Vector3(7f, 0f, 0f);

        // Reset timer
        timeElapsed = 0f;
        timerRunning = false;
        timerText.text = "A* Time: 0.00"; // Reset timer UI
    }
}
