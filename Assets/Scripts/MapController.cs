/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: Controls Game.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Room prefab;
    public Vector2 spawnPoint;

    private List<Room> _rooms;
    private List<Room> _nonDeadEnds;
    private Room[,] _roomGrid;
    public Vector2 doorwayDimensions = new Vector2(3f, 4f);
    private static Vector2Int _gridDimension = new Vector2Int(100, 100);
    private static Vector2Int _gridStart = new Vector2Int(_gridDimension.x/2, _gridDimension.y / 2);
    private static Vector2 _tileSize = new Vector2(46, 46);
    private static float _roomHeight = 5;
    private int _roomCount = 25;
    private float _hallwayPosition = .5f;

    private Vector2Int gridPositionDelta = Vector2Int.zero;

    // Start is called before the first frame update
    void Start()
    {
        prefab.doorwayDimensions = this.doorwayDimensions;

        _roomGrid = new Room[_gridDimension.x, _gridDimension.y];
        _rooms = new List<Room>(_roomCount);
        _nonDeadEnds = new List<Room>(_roomCount);

        Vector3 roomPosition = Vector3.zero;
        Vector2Int gridPosition = new Vector2Int(_gridStart.x, _gridStart.y);
        Room prevRoom = null;
        GameObject prevWall = null, wall = null;

        Vector2 spawnPointOffset = new Vector2(gridPosition.x * _tileSize.x, gridPosition.y * _tileSize.y);

        // generate random rooms
        for (int i = 0; i < _roomCount; ++i)
        {
            roomPosition = new Vector3(gridPosition.x * _tileSize.x - spawnPointOffset.x, 0f, gridPosition.y * _tileSize.y - spawnPointOffset.y);
            Vector2 wall2dDim = generateRoomSize();
            prefab.size = new Vector3(wall2dDim.x, _roomHeight, wall2dDim.y);

            Room room = Instantiate(prefab, roomPosition, Quaternion.identity);
            room.name = "Room_" + i;
            room.gridPosition.x = gridPosition.x;
            room.gridPosition.y = gridPosition.y;

            _rooms.Add(room);
            _nonDeadEnds.Add(room);

            // remember last room to connect with hallway.
            if (prevRoom != null)
            {
                wall = room.getWall(-gridPositionDelta);
                room.MakeDoorway(-gridPositionDelta, _hallwayPosition);
                MakeHallway(prevWall, wall);
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
                room.MakeDoorway(gridPositionDelta, _hallwayPosition);
                prevWall = room.getWall(gridPositionDelta);
            }

            prevRoom = room;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        float x = 4 * Mathf.Sqrt(UnityEngine.Random.Range(0, 100)) + 5;
        float y = 4 * Mathf.Sqrt(UnityEngine.Random.Range(0, 100)) + 5;
        return new Vector2(x, y);
    }

    private Room MakeHallway(GameObject wall1, GameObject wall2)
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

        prefab.size = new Vector3(dx, wall1.transform.localScale.y, dz);

        Room hallway = Instantiate(prefab, roomPosition, Quaternion.identity);

        hallway.RemoveWall(walls2Remove[0]);
        hallway.RemoveWall(walls2Remove[1]);

        hallway.transform.position = new Vector3(hallway.transform.position.x, hallway.transform.position.y - wall1.transform.localScale.y/2 - Room.wallThickness/2,hallway.transform.position.z);


        return hallway;
    }
}
