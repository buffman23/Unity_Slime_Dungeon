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

public class GameController : MonoBehaviour
{
    public Room prefab;
    public Vector2 spawnPoint;

    private List<Room> _rooms;
    private Room[,] _roomGrid;
    private Vector2Int _gridDimension = new Vector2Int(50, 50);
    private Vector2Int _gridStart = new Vector2Int(25, 25);
    private Vector2 _tileSize = new Vector2(30, 30);
    private float _roomHeight = 5;
    int room_count = 15;

    // Start is called before the first frame update
    void Start()
    {
        _roomGrid = new Room[_gridDimension.x, _gridDimension.y];

        Vector3 roomPosition = Vector3.zero;
        Vector2Int gridPosition = new Vector2Int(_gridStart.x, _gridStart.y);
        Room prevRoom;

        Vector2 spawnPointOffset = new Vector2(gridPosition.x * _tileSize.x, gridPosition.y * _tileSize.y);

        // generate random rooms
        for (int i = 0; i < room_count; ++i)
        {
            roomPosition = new Vector3(gridPosition.x * _tileSize.x - spawnPointOffset.x, 0f, gridPosition.y * _tileSize.y - spawnPointOffset.y);
            Room room = Instantiate(prefab, roomPosition, Quaternion.identity);

            Vector2 room2dDim = generateRoomSize();
            room.size = new Vector3(room2dDim.x, _roomHeight, room2dDim.y);

            _roomGrid[gridPosition.x, gridPosition.y] = room;

            List<Vector2Int> validNextTiles = validNeighborTiles(gridPosition);

            gridPosition = validNextTiles[(int)UnityEngine.Random.Range(0, validNextTiles.Count)];

            // remember last room to connect with hallway.
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

                validTiles.Add(new Vector2Int(tilePos.x + 1, tilePos.y));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x - 1, tilePos.y] == null)
            {
                validTiles.Add(new Vector2Int(tilePos.x - 1, tilePos.y));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x, tilePos.y + 1] == null)
            {
                validTiles.Add(new Vector2Int(tilePos.x, tilePos.y + 1));
            }
        }
        catch (IndexOutOfRangeException){ }

        try
        {
            if (_roomGrid[tilePos.x, tilePos.y - 1] == null)
            {
                validTiles.Add(new Vector2Int(tilePos.x, tilePos.y - 1));
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
        float x = 4 * Mathf.Sqrt(UnityEngine.Random.Range(0, 25)) + 5;
        float y = 4 * Mathf.Sqrt(UnityEngine.Random.Range(0, 25)) + 5;
        return new Vector2(x, y);
    }
}
