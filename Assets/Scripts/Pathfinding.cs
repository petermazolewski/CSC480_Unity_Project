using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    private static List<Room> FindPath(Func<Room, Room, Room[,], int, int, List<Room>> algorithm, Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY)
    {
        List<Room> path = new List<Room>();

        while (keys.Count > 0)
        {
            int path_length_to_shortest_key = int.MaxValue;
            GameObject key_to_move_to = keys[0];
            List<Room> keyPath = new List<Room>();

            foreach (GameObject key in keys)
            {
                Room key_room = FindKeyRoom(key, rooms);
                List<Room> path_to_key = algorithm(startRoom, key_room, rooms, numX, numY);

                if (path_to_key.Count < path_length_to_shortest_key)
                {
                    path_length_to_shortest_key = path_to_key.Count;
                    key_to_move_to = key;
                    keyPath = path_to_key;
                }
            }

            Room keyRoom = FindKeyRoom(key_to_move_to, rooms);
            startRoom = keyRoom;

            foreach (Room room in keyPath)
            {
                path.Add(room);
            }

            keys.Remove(key_to_move_to);
        }

        List<Room> path_to_goal = algorithm(startRoom, endRoom, rooms, numX, numY);
        foreach (Room room in path_to_goal)
        {
            path.Add(room);
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

    public static List<Room> FindPathAStar(Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY)
    {
        return FindPath(AStar, startRoom, endRoom, rooms, keys, numX, numY);
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

    public static List<Room> FindPathBFS(Room startRoom, Room endRoom, Room[,] rooms, List<GameObject> keys, int numX, int numY)
    {
        return FindPath(BFS, startRoom, endRoom, rooms, keys, numX, numY);
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
}
