using System.Collections.Generic;
using System;
using UnityEngine;

public class AStarAgent : Agent, IKeyObserver
{
    protected override void Start()
    {
        base.Start();
        timerText.text = "A* Time: 0.00";
        keyAmount.text = "Key: 0";

        BFSAgent obsbfs = FindAnyObjectByType<BFSAgent>();
        if (obsbfs != null)
        {
            AddObserver(obsbfs);
        }

        Player obsplayer = FindAnyObjectByType<Player>();
        if (obsplayer != null)
        {
            AddObserver(obsplayer);
        }
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

        if (!timerRunning)
        {
            timerRunning = true;
        }

        path = Pathfinding.FindPathAStar(currentRoom, rooms, keyObjects, 10, 15);
        pathIndex = 0;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.tag == "Keys")
        {
            currentRoom = GetCurrentRoom();
            doorRoom = FindFirstObjectByType<GenerateMaze>().GetDoorRoom();

            if (keys >= requiredKeys || AllKeysCollected()) {
                path = Pathfinding.AStar(currentRoom, doorRoom, rooms, 10, 15);
            } else {
                path = Pathfinding.FindPathAStar(currentRoom, rooms, keyObjects, 10, 15);
            }
            pathIndex = 0;
        }
    }

    public void OnKeyCollected(GameObject collision)
    {
        keyObjects.Remove(collision);

        if (path == null) return;

        keyObjects = FindFirstObjectByType<GenerateMaze>().GetSpawnedKeys();

        currentRoom = GetCurrentRoom();
        doorRoom = FindFirstObjectByType<GenerateMaze>().GetDoorRoom();

        if (keys >= requiredKeys || AllKeysCollected()) {
            path = Pathfinding.AStar(currentRoom, doorRoom, rooms, 10, 15);
            pathIndex = 0;
        } else {
            path = Pathfinding.FindPathAStar(currentRoom, rooms, keyObjects, 10, 15);
            pathIndex = 0;
        }
    }

    override public void ResetAgent()
    {
        path = null;
        pathIndex = 0;
        isAI = false;

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(7f, 0f, 0f);
        exiting = false;
        keysCollected = false;

        // Reset timer
        timeElapsed = 0f;
        timerRunning = false;
        timerText.text = "A* Time: 0.00"; // Reset timer UI
        keyAmount.text = "Key: 0";
    }
}
