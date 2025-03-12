    using UnityEngine;

    public class AStarAgent : Agent
    {
        protected override void Start()
        {
            base.Start();
            timerText.text = "A* Time: 0.00";
            keyAmount.text = "Key: 0";
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

            if (!timerRunning)
            {
                timerRunning = true; // Start the timer when agent starts moving
            }

            // Pass 3 as maxKeys to only look for the closest 3 keys
            path = Pathfinding.FindPathAStar(currentRoom, doorRoom, rooms, keyObjects, 10, 15, 3);
            pathIndex = 0;
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
