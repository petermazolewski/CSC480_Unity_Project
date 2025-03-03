using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<Room> FindPath(Room startRoom, Room endRoom, Room[,] rooms, int numX, int numY)
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

        return new List<Room>(); // Return empty list if no path found
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
