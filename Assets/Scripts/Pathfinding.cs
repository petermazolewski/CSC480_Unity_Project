using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour , IKeyObserver
{
    private Room targetKeyRoom;
    private Room startRoom;
    private Room endRoom;
    private Room[,] rooms;
    private List<GameObject> keyObjects;
    private int numX, numY;

    private static List<Room> FindPath(Func<Room, Room, Room[,], int, int, List<Room>> algorithm, Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY, int maxKeys = 3)
    {
        List<GameObject> keysCopy = new List<GameObject>(keys);
        List<Room> path = new List<Room>();

        int keysCollected = 0;

        while (keysCopy.Count > 0 && keysCollected < maxKeys) // Limit to 3 keys
        {
            int pathLengthToShortestKey = int.MaxValue;
            GameObject keyToMoveTo = keysCopy[0];
            List<Room> keyPath = new List<Room>();

            foreach (GameObject key in keysCopy)
            {
                Room currentkeyRoom = FindKeyRoom(key, rooms); // Get the key room
                List<Room> pathToKey = algorithm(startRoom, currentkeyRoom, rooms, numX, numY);

                if (pathToKey.Count < pathLengthToShortestKey)
                {
                    pathLengthToShortestKey = pathToKey.Count;
                    keyToMoveTo = key;
                    keyPath = pathToKey;
                }
            }

            Room keyRoom = FindKeyRoom(keyToMoveTo, rooms); // Get the room for the collected key
            startRoom = keyRoom; // Update start room to the key room

            foreach (Room room in keyPath)
            {
                path.Add(room);
            }

            keysCopy.Remove(keyToMoveTo); // Remove the collected key from the list
            keysCollected++; // Increment the number of collected keys
        }

        // After collecting the keys, proceed to the door
        if (keysCollected >= maxKeys)
        {
            List<Room> pathToGoal = algorithm(startRoom, endRoom, rooms, numX, numY);
            foreach (Room room in pathToGoal)
            {
                path.Add(room);
            }
        }

        return path;
    }


    public static List<Room> AStar(Room startRoom, Room endRoom, Room[,] rooms, int numX, int numY)
    {
        PriorityQueue<Room> openSet = new PriorityQueue<Room>();
        HashSet<Room> closedSet = new HashSet<Room>();
        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();
        Dictionary<Room, float> gScore = new Dictionary<Room, float>();
        Dictionary<Room, float> fScore = new Dictionary<Room, float>();

        openSet.Enqueue(startRoom, 0);
        gScore[startRoom] = 0;
        fScore[startRoom] = Heuristic(startRoom, endRoom);

        while (openSet.Count > 0)
        {
            Room current = openSet.Dequeue();

            if (current == endRoom)
                return ReconstructPath(cameFrom, current);

            closedSet.Add(current);

            foreach (Room neighbor in current.GetConnectedRooms(rooms, numX, numY))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endRoom);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return new List<Room>();
    }

    public static List<Room> FindPathAStar(Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY, int maxKeys)
    {
        return FindPath(AStar, startRoom, endRoom, rooms, keys, numX, numY, maxKeys);
    }

    public static List<Room> BFS(Room startRoom, Room endRoom, Room[,] rooms, int numX, int numY)
    {
        Queue<Room> openSet = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();
        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();

        openSet.Enqueue(startRoom);
        visited.Add(startRoom);

        while (openSet.Count > 0)
        {
            Room node = openSet.Dequeue();
            if (node == endRoom)
            {
                return ReconstructPath(cameFrom, node);
            }

            foreach (Room neighbor in node.GetConnectedRooms(rooms, numX, numY))
            {
                if (visited.Contains(neighbor))
                    continue;

                openSet.Enqueue(neighbor);
                visited.Add(neighbor);
                cameFrom[neighbor] = node;
            }
        }

        return new List<Room>();
    }

    private static Room FindKeyRoom(GameObject key, Room[,] rooms)
    {
        foreach (Room room in rooms)
        {
            if (Vector3.Distance(room.transform.position, key.transform.position) < 0.5f)
                return room;
        }
        return null;
    }

    public static List<Room> FindPathBFS(Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY, int maxKeys)
    {
        return FindPath(BFS, startRoom, endRoom, rooms, keys, numX, numY, maxKeys);
    }

    private static float Heuristic(Room a, Room b)
    {
        return Mathf.Abs(a.Index.x - b.Index.x) + Mathf.Abs(a.Index.y - b.Index.y);
    }

    private static List<Room> ReconstructPath(Dictionary<Room, Room> cameFrom, Room current)
    {
        List<Room> path = new List<Room> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    public void Initialize(Room start, Room end, Room[,] roomGrid, List<GameObject> keys, int x, int y)
    {
        startRoom = start;
        endRoom = end;
        rooms = roomGrid;
        keyObjects = keys;
        numX = x;
        numY = y;
    }

    public void OnKeyCollected(List<GameObject> updatedKeys)
    {
        keyObjects = updatedKeys;

        if (targetKeyRoom != null && !keyObjects.Contains(targetKeyRoom.gameObject))
        {
            // Recalculate path if the targeted key is gone
            List<Room> newPath = FindPath(AStar, startRoom, endRoom, rooms, keyObjects, numX, numY);
            Debug.Log("Path recalculated due to key collection.");
        }
    }
}
