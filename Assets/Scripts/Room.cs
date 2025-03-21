using System.Collections.Generic;
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
    private GameObject topWall;
    [SerializeField]
    private GameObject rightWall;
    [SerializeField]
    private GameObject bottomWall;
    [SerializeField]
    private GameObject leftWall;

    [SerializeField]
    private GameObject keyPrefab; // Reference to the key prefab

    private GameObject spawnedKey; // Track spawned key

    private Dictionary<Directions, GameObject> walls = new Dictionary<Directions, GameObject>();

    public Vector2Int Index { get; set; }

    public bool visited { get; set; } = false;

    private Dictionary<Directions, bool> dirflags = new Dictionary<Directions, bool>();

    private void Start()
    {
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;
    }

    private void SetActive(Directions dir, bool flag)
    {
        if (walls.ContainsKey(dir))
        {
            walls[dir].SetActive(flag);
        }
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        dirflags[dir] = flag;
        SetActive(dir, flag);
    }


    public void SpawnKey()
    {
        if (keyPrefab != null && spawnedKey == null)
        {
            spawnedKey = Instantiate(keyPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void RemoveKey()
    {
        if (spawnedKey != null)
        {
            Destroy(spawnedKey);
            spawnedKey = null;
        }
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
