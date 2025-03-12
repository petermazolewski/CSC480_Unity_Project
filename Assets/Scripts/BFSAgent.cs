using System.Collections.Generic;
using System;
using UnityEngine;

public class BFSAgent : Agent, IKeyObserver
{
    protected override void Start()
    {
        base.Start();
        timerText.text = "BFS Time: 0.00";
        keyAmount.text = "Key: 0";

        AStarAgent obsa = FindAnyObjectByType<AStarAgent>();
        if (obsa != null)
        {
            AddObserver(obsa);
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
            timerText.text = "BFS Time: " + timeElapsed.ToString("F2") + " seconds"; // Update the timer UI
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

        path = Pathfinding.FindPathBFS(currentRoom, rooms, keyObjects, 10, 15);
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
                path = Pathfinding.BFS(currentRoom, doorRoom, rooms, 10, 15);
            } else {
                path = Pathfinding.FindPathBFS(currentRoom, rooms, keyObjects, 10, 15);
            }
            pathIndex = 0;
        }
    }

    public void OnKeyCollected(GameObject collision)
    {
        keyObjects.Remove(collision);

        if (path == null) return;

        currentRoom = GetCurrentRoom();
        doorRoom = FindFirstObjectByType<GenerateMaze>().GetDoorRoom();

        if (keys >= requiredKeys || AllKeysCollected()) {
            path = Pathfinding.BFS(currentRoom, doorRoom, rooms, 10, 15);
        } else {
            path = Pathfinding.FindPathBFS(currentRoom, rooms, keyObjects, 10, 15);
        }
        pathIndex = 0;
    }


    override public void ResetAgent()
    {
        path = null;
        pathIndex = 0;
        isAI = false;

        // Reset agent's state
        keys = 0;
        transform.position = new Vector3(0f, 98f, 0f);
        exiting = false;
        keysCollected = false;

        // Reset timer
        timeElapsed = 0f;
        timerRunning = false;
        timerText.text = "BFS Time: 0.00"; // Reset timer UI\
        keyAmount.text = "Key: 0";
    }
}
