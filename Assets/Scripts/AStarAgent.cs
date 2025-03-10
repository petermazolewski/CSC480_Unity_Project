using UnityEngine;

public class AStarAgent : Agent
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
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
}
