using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;
    public GameObject door;
    
    public GameObject keyPrefab; //key prefab

    public int totalKeysRequired = 3; //number of keys

    private GameObject spawnedDoor; //store door reference

    private List<GameObject> spawnedKeys = new List<GameObject>(); // Store keys

    // The grid
    Room[,] rooms = null;

    [SerializeField]
    int numX = 10;
    int numY = 10;

    // The room width and height
    float roomWidth;
    float roomHeight;

    // The stack for backtracking
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers = roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach(SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(minBounds, ren.bounds.min);
            maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(numX * (roomWidth - 1) / 2, numY * (roomHeight - 1) / 2, -100.0f);

        float min_value = Mathf.Min(numX * (roomWidth - 1), numY * (roomHeight - 1));
        Camera.main.orthographicSize = min_value * 0.75f;
    }

    private void Start()
    {
        //keys = 0;
        //keyAmount.txt = "Keys: " + keys; 
        GetRoomSize();

        rooms = new Room[numX, numY];

        for(int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                GameObject room = Instantiate(roomPrefab, new Vector3(i * roomWidth, j * roomHeight, 0.0f), Quaternion.identity);

                room.name = "Room_" + i.ToString() + "_ " + j.ToString();
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();


    }

    private void RemoveRoomWall(int x, int y, Room.Directions dir)
    {
        if(dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }


        Room.Directions opp = Room.Directions.NONE;
        switch (dir)
        {
            case Room.Directions.TOP:
                if(y < numY - 1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if(x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if(y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if(x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if(opp != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours = new List<Tuple<Room.Directions, Room>>();

        foreach(Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if(y < numY - 1)
                    {
                        ++y;
                        if(!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.TOP, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.RIGHT, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.BOTTOM, rooms[x, y]));
                        }
                    }
                    break;
                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.LEFT, rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }

    private bool GenerateStep()
    {
        if(stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if(neighbours.Count != 0)
        {
            var index = 0;
            if(neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        if(generating) return;

        Reset();

        RemoveRoomWall(0, 0, Room.Directions.BOTTOM);

        RemoveRoomWall(numX - 1, numY - 1, Room.Directions.RIGHT);

        Vector3 doorPosition = rooms[numX - 1, numY - 1].transform.position;
        doorPosition.x += (roomWidth - 1)/ 2;
        Instantiate(door, doorPosition, Quaternion.identity);
        DespawnKeys();
        SpawnKeys();

        stack.Push(rooms[0, 0]);

        StartCoroutine(Coroutine_Generate());
    }
    private void DespawnKeys()
    {
        foreach (GameObject key in spawnedKeys)
        {
            if (key != null)
            {
                Destroy(key);
            }
        }
        spawnedKeys.Clear();
    }


    private void SpawnKeys()
    {
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        for(int i = 0; i < totalKeysRequired; i++)
        {
            int x, y;
            do
            {
                x = UnityEngine.Random.Range(0, numX);
                y = UnityEngine.Random.Range(0, numY);
            }
            while(usedPositions.Contains(new Vector2Int(x, y)) || (x == 0 && y == 0)); // Avoid start position

            usedPositions.Add(new Vector2Int(x, y));
            Vector3 keyPosition = rooms[x, y].transform.position;
            GameObject newKey = Instantiate(keyPrefab, keyPosition, Quaternion.identity);
            spawnedKeys.Add(newKey);
            //Instantiate(keyPrefab, keyPosition, Quaternion.identity);
        }
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;
        bool flag = false;
        while(!flag)
        {
            flag = GenerateStep();
            // yield return new WaitForSeconds(0.05f);
            yield return null;
        }

        generating = false;
    }

    private void Reset()
    {
        for(int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!generating)
            {
                CreateMaze();
                GameObject aStarAgentObject = GameObject.Find("AStarAgent");
                if (aStarAgentObject != null)
                {
                    AStarAgent aStarAgent = aStarAgentObject.GetComponent<AStarAgent>();
                    if (aStarAgent != null)
                    {
                        aStarAgent.ResetAgent();
                        aStarAgent.isAI = true;
                    }
                }

                GameObject bfsAgentObject = GameObject.Find("BFSAgent");
                if (bfsAgentObject != null)
                {
                    BFSAgent bfsAgent = bfsAgentObject.GetComponent<BFSAgent>();
                    if (bfsAgent != null)
                    {
                        bfsAgent.ResetAgent();
                        bfsAgent.isAI = true;
                    }
                }

                GameObject playerObject = GameObject.Find("Player");
                if (playerObject != null)
                {
                    Player player = playerObject.GetComponent<Player>();
                    if (player != null)
                    {
                        player.ResetPlayer();
                    }
                }
            }
        }
    }

    public Room[,] GetRooms()
    {
        return rooms;
    }

    public Room GetDoorRoom()
    {
        return rooms[numX - 1, numY - 1]; // Assuming the door is always at the last room
    }

    public List<GameObject> GetSpawnedKeys()
    {
        return spawnedKeys;
    }
}
