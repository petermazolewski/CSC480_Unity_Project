using UnityEngine;

public class AStarAgent : Agent
{
    protected override void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
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

        path = Pathfinding.FindPathAStar(currentRoom, doorRoom, rooms, keyObjects, 10, 10);
        pathIndex = 0;
    }

    override public void ResetAgent()
    {
        // Reset pathfinding state
        path = null;
        pathIndex = 0;
        isAI = false;

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(7f, 0f, 0f);
    }
}
