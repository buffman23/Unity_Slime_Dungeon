/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: This class controls map generation
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapController : MonoBehaviour
{
    public static MapController instance;
    public Room room;
    private Room _roomPrefab;
    private Hallway _hallwayPrefab;

    public Vector2 spawnPoint;

    public Vector2 doorwayDimensions = new Vector2(3f, 4f);

    public bool spawnRoom;

    private List<Room> _rooms;
    private List<Room> _nonDeadEnds;
    private Room[,] _roomGrid;
    private static Vector2Int _gridDimension = new Vector2Int(100, 100);
    private static Vector2Int _gridStart = new Vector2Int(_gridDimension.x/2, _gridDimension.y / 2);
    private static Vector2 _tileSize = new Vector2(46, 46);
    private static Vector2 _spawnPointOffset = new Vector2(_gridStart.x * _tileSize.x, _gridStart.y * _tileSize.y);
    private static float _roomHeight = 15;
    private int _roomCount = 25;
    private float _hallwayPosition = .5f;

    private Room[] _activeRooms = new Room[5];

    private int ticks = 0;

    private PlayerController _player;

    private Vector2Int gridPositionDelta = Vector2Int.zero;


    public NavMeshSurface surface;

    // Start is called before the first frame update

    void Start()
    {
        if (instance == null)
            instance = this;

        InitReferences();

        InitRoomSpawnOptions();

        _roomPrefab.doorwayDimensions = this.doorwayDimensions;

        _roomGrid = new Room[_gridDimension.x, _gridDimension.y];
        _rooms = new List<Room>(_roomCount);
        _nonDeadEnds = new List<Room>(_roomCount);

        Vector3 roomPosition = Vector3.zero;
        Vector2Int gridPosition = new Vector2Int(_gridStart.x, _gridStart.y);
        Room prevRoom = null;
        GameObject prevWall = null, wall = null;
        GameObject map = new GameObject();
        map.name = "map";

        // generate random rooms
        for (int i = 0; i < _roomCount; ++i)
        {
            roomPosition = new Vector3(gridPosition.x * _tileSize.x - _spawnPointOffset.x, 0f, gridPosition.y * _tileSize.y - _spawnPointOffset.y);
            Vector2 wall2dDim = generateRoomSize();

            if (spawnRoom && i == 0)
            {
                wall2dDim = new Vector2(8, 8);
            }

            _roomPrefab.size = new Vector3(wall2dDim.x, _roomHeight, wall2dDim.y);
            

            room = Instantiate(_roomPrefab, roomPosition, Quaternion.identity);
            //room.ga
            room.transform.SetParent(map.transform);
            room.name = "Room_" + i;
            room.gridPosition.x = gridPosition.x;
            room.gridPosition.y = gridPosition.y;

            _rooms.Add(room);
            _nonDeadEnds.Add(room);

            // remember last room to connect with hallway.
            if (prevRoom != null)
            {
                prevRoom.nextRoom = room;
                room.previousRoom = prevRoom;

                wall = room.getWall(-gridPositionDelta);
                room.MakeDoorway(-gridPositionDelta, _hallwayPosition, false);
                Hallway hallway = MakeHallway(prevWall, wall);
                hallway.transform.SetParent(room.transform);
            }

            // Last room wont have another room to make so break early
            if (i == _roomCount - 1)
            {
                break;
            }

            _roomGrid[gridPosition.x, gridPosition.y] = room;

            List<Vector2Int> validNextTiles = null;

            for(int j = 1; true; ++j)
            {
                validNextTiles = validNeighborTiles(gridPosition);

                if (validNextTiles.Count == 0)
                {
                    room.deadEnd = true;
                    _rooms.Remove(room);
                    room = _nonDeadEnds[_nonDeadEnds.Count - j];
                    gridPosition.x = room.gridPosition.x;
                    gridPosition.y = room.gridPosition.y;
                }
                else
                {
                    break;
                }
            }

            gridPositionDelta = validNextTiles[(int)UnityEngine.Random.Range(0, validNextTiles.Count)];
            gridPosition += gridPositionDelta;

            if (i != _roomCount - 1)
            {
                room.MakeDoorway(gridPositionDelta, _hallwayPosition, true);
                prevWall = room.getWall(gridPositionDelta);
            }

            prevRoom = room;
        }

        // populate rooms with structures and deactivate them after.
        // skip first room if spawRoom
        int startIdx = 0;
        if (spawnRoom)
        {
            _rooms[0].GenerateKey(new List<GameObject>(0));
            startIdx = 1;
        }
        for(int i = startIdx; i < _rooms.Count; ++i)
        {
            room = _rooms[i];
            room.GenerateTiles();
            room.gameObject.SetActive(false);

        }

        //setRoomGridsVisible(true);
    }
    public void HandleDoorOpened()
    {
        surface.BuildNavMesh();
    }

    private void InitReferences()
    {
        _roomPrefab = Resources.Load<Room>("Prefabs/Room");
        _hallwayPrefab = Resources.Load<Hallway>("Prefabs/Hallway");
        _player = PlayerController.instance;
        surface = gameObject.AddComponent<NavMeshSurface>();
    }

    private void InitRoomSpawnOptions()
    {
        SpawnOption[] spawnOptions = Resources.LoadAll<SpawnOption>("Prefabs/SpawnOptions");

        Room.smallSpawnOptions = new List<SpawnOption>();
        Room.largeSpawnOptions = new List<SpawnOption>();
        Room.xLargeSpawnOptions = new List<SpawnOption>();

        foreach (SpawnOption so in spawnOptions)
        {
            if (!so.includeInTilePool)
                continue;

            if (so.tag.Equals("SpawnOptionS"))
            {
                Room.smallSpawnOptions.Add(so);
            }else if (so.tag.Equals("SpawnOptionL"))
            {
                Room.largeSpawnOptions.Add(so);
            } else if (so.tag.Equals("SpawnOptionX"))
            {
                Room.xLargeSpawnOptions.Add(so);
            }
        }
    }

    /*
     * Authors: Ryan Coughlin
     * Desc: Gives valid neighboring positions on _roomGrid to place a room.
     * Params: tilePos - Position to look for adjacent tiles that are open.
     * Returns: List of all open adjacent tiles.
     */
    private List<Vector2Int> validNeighborTiles(Vector2Int tilePos)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>(4);

        try
        {
            if (_roomGrid[tilePos.x + 1, tilePos.y] == null)
            {

                validTiles.Add(new Vector2Int(1, 0));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x - 1, tilePos.y] == null)
            {
                validTiles.Add(new Vector2Int(-1, 0));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x, tilePos.y + 1] == null)
            {
                validTiles.Add(new Vector2Int(0, 1));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x, tilePos.y - 1] == null)
            {
                validTiles.Add(new Vector2Int(0, -1));
            }
        }
        catch (IndexOutOfRangeException){ }

        return validTiles;
    }

    /*
     * Authors: Ryan Coughlin
     * Desc: Generated a Vector2 with random x,y components and a skewed distribution.
     *       This is to more often gererate larger rooms.
     */
    private Vector2 generateRoomSize()
    {
        float x = 5 * Mathf.Sqrt(UnityEngine.Random.Range(0, 50)) + Room.LARGE_GRID_SIZE;
        float y = 5 * Mathf.Sqrt(UnityEngine.Random.Range(0, 50)) + Room.LARGE_GRID_SIZE;

        x = x - x % Room.LARGE_GRID_SIZE;
        y = y - y % Room.LARGE_GRID_SIZE;
        return new Vector2(x, y);
    }

    private Hallway MakeHallway(GameObject wall1, GameObject wall2)
    {
        Vector3 roomPosition = Vector3.zero;
        roomPosition = (wall1.transform.position + wall2.transform.position) / 2;

        float dx = Mathf.Abs(wall1.transform.position.x - wall2.transform.position.x);
        float dz = Mathf.Abs(wall1.transform.position.z - wall2.transform.position.z);

        // whichever dim is 0, should be hallway width
        int[] walls2Remove = new int[2];

        if (dx == 0)
        {
            dx = doorwayDimensions.y - Room.wallThickness - .01f; 
            walls2Remove[0] = Room.EAST_WALL;
            walls2Remove[1] = Room.WEST_WALL;
            dz += Room.wallThickness - .01f;
        }
        else
        {
            dz = doorwayDimensions.y - Room.wallThickness - .01f;
            walls2Remove[0] = Room.NORTH_WALL;
            walls2Remove[1] = Room.SOUTH_WALL;
            dx += Room.wallThickness - .01f;

        }

        _hallwayPrefab.size = new Vector3(dx, wall1.transform.localScale.y, dz);

        Hallway hallway = Instantiate(_hallwayPrefab, roomPosition, Quaternion.identity);

        hallway.RemoveWall(walls2Remove[0]);
        hallway.RemoveWall(walls2Remove[1]);

        hallway.transform.position = new Vector3(hallway.transform.position.x, hallway.transform.position.y - wall1.transform.localScale.y/2 - Room.wallThickness/2,hallway.transform.position.z);


        return hallway;
    }


    public void setRoomGridsVisible(bool b)
    {
        foreach(Room room in _rooms)
        {
            room.SetGridVisible(b);
        }
    }

    private void Update()
    {
        if (ticks++ % 100 != 0)
            return;

        float playerLocationX = _player.transform.position.x;
        float playerLocationY = _player.transform.position.z;

        int xGridPos = (int)((_spawnPointOffset.x + playerLocationX + _tileSize.x/2f) / _tileSize.x);
        int yGridPos = (int)((_spawnPointOffset.y + playerLocationY + _tileSize.y/2f) / _tileSize.y);

        //Debug.Log("(" + xGridPos + ", " + yGridPos + ")");
        
        Room room = _roomGrid[xGridPos, yGridPos];

        if (room != null) {
            if (room != _activeRooms[2])
            {
                // update players current room reference
                PlayerController.instance.currentRoom = room;

                Room[] newActiveRooms = new Room[5];

                if (room.previousRoom != null)
                {
                    newActiveRooms[1] = room.previousRoom;

                    if (room.previousRoom.previousRoom != null)
                    {
                        newActiveRooms[0] = room.previousRoom.previousRoom;
                    }
                }

                newActiveRooms[2] = room;

                if (room.nextRoom != null)
                {
                    newActiveRooms[3] = room.nextRoom;

                    if (room.nextRoom.nextRoom != null)
                    {
                        newActiveRooms[4] = room.nextRoom.nextRoom;
                    }
                }

                for (int i = 0; i < _activeRooms.Length; ++i)
                {
                    if (_activeRooms[i] != null)
                    {
                        bool contains = false;
                        for (int j = 0; j < newActiveRooms.Length; ++j)
                        {
                            if (_activeRooms[i] == newActiveRooms[j])
                            {
                                contains = true;
                                break;
                            }
                        }

                        if (!contains)
                        {
                            _activeRooms[i].gameObject.SetActive(false);
                        }
                    }
                }

                foreach (Room r in newActiveRooms)
                {
                    if (r != null)
                    {
                        r.gameObject.SetActive(true);
                    }
                }

                _activeRooms = newActiveRooms;

                // rebuild nav mesh for active rooms
                surface.BuildNavMesh();
            }
        }
    }
}
