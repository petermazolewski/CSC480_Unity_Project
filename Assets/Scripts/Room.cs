using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE
    }

    [SerializeField]
    GameObject topWall;
    [SerializeField]
    GameObject rightWall;
    [SerializeField]
    GameObject bottomWall;
    [SerializeField]
    GameObject leftWall;

    Dictionary<Directions, GameObject> walls = new Dictionary<Directions, GameObject>();

    public Vector2Int Index
    {
        get;
        set;
    }

    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirflags = new Dictionary<Directions, bool>();

    private void Start()
    {
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;
    }

    private void SetActive(Directions dir, bool flag)
    {
        walls[dir].SetActive(flag);
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        dirflags[dir] = flag;
        SetActive(dir, flag);
    }

    public List<Room> GetConnectedRooms(Room[,] rooms, int numX, int numY)
    {
    List<Room> neighbors = new List<Room>();

    if (!dirflags[Directions.TOP] && Index.y < numY - 1)
        neighbors.Add(rooms[Index.x, Index.y + 1]);
    if (!dirflags[Directions.RIGHT] && Index.x < numX - 1)
        neighbors.Add(rooms[Index.x + 1, Index.y]);
    if (!dirflags[Directions.BOTTOM] && Index.y > 0)
        neighbors.Add(rooms[Index.x, Index.y - 1]);
    if (!dirflags[Directions.LEFT] && Index.x > 0)
        neighbors.Add(rooms[Index.x - 1, Index.y]);

    return neighbors;
    }


}
