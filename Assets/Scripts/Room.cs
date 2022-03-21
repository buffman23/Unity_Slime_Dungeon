/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: Room prefab. Generates rectangular room.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<SpawnOption> smallSpawnOptions, largeSpawnOptions, xLargeSpawnOptions;

    public Vector3 size;

    public bool deadEnd = false;

    public Vector2Int gridPosition = Vector2Int.zero;

    public Vector2 doorwayDimensions = new Vector2(3f, 4f);

    public static float wallThickness = 1f;

    public const int NORTH_WALL = 0, EAST_WALL = 1, SOUTH_WALL = 2, WEST_WALL = 3;

    private GameObject[] _walls = new GameObject[4];

    private GameObject _floor;

    private GameObject _ceiling;

    private Material _roomMaterial, _smallTileMaterial, _largeTileMaterial, _xLargeTileMaterial;

    private List<GameObject> _spawnObjects;

    private int[,] _smallSpawnGrid, _largeSpawnGrid, _xLargeSpawnGrid;

    private List<GameObject> _drawGridObjects;

   

    public const int SMALL_GRID_SIZE = 1,  LARGE_GRID_SIZE = 8,  XLARGE_GRID_SIZE = 16;

    public const int EMPTY = -1, TAKEN = -2, FOO = -3;

    private void Awake()
    {
        initReferences();

        _roomMaterial = Resources.Load<Material>("materials/Room");

        _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _floor.transform.SetParent(transform);
        _floor.transform.localPosition = new Vector3(0, 0, 0);
        _floor.transform.localScale = new Vector3(size.x, wallThickness, size.z);
        _floor.name = transform.gameObject.name + "__floor";
        _floor.GetComponent<Renderer>().material = _roomMaterial;
        _floor.layer = LayerMask.NameToLayer("Ground");

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(size.x / 2f + wallThickness / 2f, size.y / 2f + wallThickness / 2f, 0);
        wall.transform.localScale = new Vector3(wallThickness, size.y, size.z);
        wall.name = transform.gameObject.name + "_NorthWall";
        _walls[NORTH_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(-size.x / 2f - wallThickness / 2f, size.y / 2f + wallThickness / 2f, 0);
        wall.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(wallThickness, size.y, size.z);
        wall.name = transform.gameObject.name + "_SouthWall";
        _walls[SOUTH_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(0, size.y / 2f + wallThickness / 2f, -size.z / 2f - wallThickness / 2f);
        wall.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(wallThickness, size.y, size.x);
        wall.name = transform.gameObject.name + "EastWall";
        _walls[EAST_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(0, size.y / 2f + wallThickness / 2f, size.z / 2f + wallThickness / 2f);
        wall.transform.Rotate(new Vector3(0f, 270f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(wallThickness, size.y, size.x);
        wall.name = transform.gameObject.name + "_WestWall";
        _walls[WEST_WALL] = wall;

        foreach (GameObject w in _walls)
        {
            w.GetComponent<Renderer>().material = _roomMaterial;
            w.layer = LayerMask.NameToLayer("Ground");
        }

        if (this is Hallway)
            return;

        Vector2Int gridDim = new Vector2Int((int)(size.x / XLARGE_GRID_SIZE), (int)(size.z / XLARGE_GRID_SIZE));

        _xLargeSpawnGrid = new int[gridDim.x, gridDim.y];
        for (int i = 0; i < gridDim.x; ++i)
        {
            for (int j = 0; j < gridDim.y; ++j)
            {
                _xLargeSpawnGrid[i, j] = Room.EMPTY;
            }
        }

        gridDim = new Vector2Int((int)(size.x / LARGE_GRID_SIZE), (int)(size.z / LARGE_GRID_SIZE));

        _largeSpawnGrid = new int[gridDim.x, gridDim.y];
        for(int i = 0; i < gridDim.x; ++i)
        {
            for(int j = 0; j < gridDim.y; ++j)
            {
                _largeSpawnGrid[i, j] = Room.EMPTY;
            }
        }

        gridDim = new Vector2Int((int)(size.x / SMALL_GRID_SIZE), (int)(size.z / SMALL_GRID_SIZE));

        _smallSpawnGrid = new int[gridDim.x , gridDim.y];
        for (int i = 0; i < gridDim.x; ++i)
        {
            for (int j = 0; j < gridDim.y; ++j)
            {
                _smallSpawnGrid[i, j] = Room.EMPTY;
            }
        }

        if(_xLargeSpawnGrid.GetLength(0) != 0 && _xLargeSpawnGrid.GetLength(1) != 0)
            generateTileNums(_xLargeSpawnGrid, _largeSpawnGrid, xLargeSpawnOptions, XLARGE_GRID_SIZE/LARGE_GRID_SIZE);
        generateTileNums(_largeSpawnGrid, null, largeSpawnOptions, LARGE_GRID_SIZE / SMALL_GRID_SIZE);
        generateTileNums(_smallSpawnGrid, null, smallSpawnOptions, 1);


        // Temportary testing code
        //MakeDoorway(_walls[NORTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[EAST_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[SOUTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[WEST_WALL], Random.Range(0f, 1f));
    }
    // Start is called before the first frame update
    private void initReferences()
    {
        _smallTileMaterial = Resources.Load<Material>("Materials/SmallTile");
        _largeTileMaterial = Resources.Load<Material>("Materials/LargeTile");
        _xLargeTileMaterial = Resources.Load<Material>("Materials/XLargeTile");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject getWall(int side)
    {
        return _walls[side];
    }

    public GameObject getWall(Vector2 side)
    {
        int wall_const = NORTH_WALL;

        if (side.x == 1)
        {
            wall_const = NORTH_WALL;
        }
        else if (side.x == -1)
        {
            wall_const = SOUTH_WALL;
        }
        else if (side.y == 1)
        {
            wall_const = WEST_WALL;
        }
        else if (side.y == -1)
        {
            wall_const = EAST_WALL;
        }

        return getWall(wall_const);
    }

    public void MakeDoorway(Vector2 side, float position)
    {
        int wall_const = NORTH_WALL;

        if (side.x == 1)
        {
            wall_const = NORTH_WALL;
        }
        else if (side.x == -1)
        {
            wall_const = SOUTH_WALL;
        }
        else if (side.y == 1)
        {
            wall_const = WEST_WALL;
        }
        else if (side.y == -1)
        {
            wall_const = EAST_WALL;
        }

        MakeDoorway(_walls[wall_const], position);
    }

    public void MakeDoorway(int side, float position)
    {
        MakeDoorway(_walls[side], position);
    }

    /*
     * Authors: Ryan
     * Desc: Adds a doorway to a wall given a relative wall position.
     * Params: wall - The wall to add a doorway to.
     *         position - Relative wall position [0-1].
     */
    private void MakeDoorway(GameObject wall, float position)
    {
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.layer = LayerMask.NameToLayer("Ground");
        top.transform.localScale = new Vector3(wallThickness, wall.transform.localScale.y - doorwayDimensions.y, wall.transform.localScale.z);
        top.transform.rotation = wall.transform.rotation;
        top.transform.SetParent(wall.transform);
        top.transform.localPosition = new Vector3(0, .5f - top.transform.localScale.y/2f, 0);
        top.name = wall.name + "_TopWall";
        top.GetComponent<Renderer>().material = _roomMaterial;

        Vector2 scale = top.GetComponent<Renderer>().material.mainTextureScale;
        top.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * top.transform.localScale.z, scale.y * top.transform.localScale.y);


        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left.transform.localScale = new Vector3(wallThickness, wall.transform.localScale.y - top.transform.localScale.x, 
            wall.transform.localScale.z * position - doorwayDimensions.x/2f);
        left.transform.rotation = wall.transform.rotation;
        left.transform.SetParent(wall.transform);
        left.transform.localPosition = new Vector3(0, -.5f + left.transform.localScale.y/2f,
            .5f - left.transform.localScale.z/2f);
        left.name = wall.name + "_LeftWall";
        left.GetComponent<Renderer>().material = _roomMaterial;

        scale = left.GetComponent<Renderer>().material.mainTextureScale;
        left.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * left.transform.localScale.z, scale.y * left.transform.localScale.y);

        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right.transform.localScale = new Vector3(wallThickness, wall.transform.localScale.y - top.transform.localScale.x,
            wall.transform.localScale.z * (1 - position) - doorwayDimensions.x / 2f);
        right.transform.rotation = wall.transform.rotation;
        right.transform.SetParent(wall.transform);
        right.transform.localPosition = new Vector3(0, -.5f + right.transform.localScale.y / 2f,
            -.5f + right.transform.localScale.z / 2f);
        right.name = wall.name + "_RightWall";
        right.GetComponent<Renderer>().material = _roomMaterial;

        scale = right.GetComponent<Renderer>().material.mainTextureScale;
        right.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * right.transform.localScale.z, scale.y * right.transform.localScale.y);

        Destroy(wall.GetComponent<MeshRenderer>());
        Destroy(wall.GetComponent<BoxCollider>());
    }

    public void RemoveWall(int side)
    {
        Destroy(_walls[side]);
    }
    public void RemoveWall(Vector2 side)
    { 
        int wall_const = NORTH_WALL;

        if (side.x == 1)
        {
            wall_const = NORTH_WALL;
        }
        else if (side.x == -1)
        {
            wall_const = SOUTH_WALL;
        }
        else if (side.y == 1)
        {
            wall_const = WEST_WALL;
        }
        else if (side.y == -1)
        {
            wall_const = EAST_WALL;
        }
        RemoveWall(wall_const);
    }

    private void generateTileNums(int[,] grid, int[,] nextGrid, List<SpawnOption> spawnOptions, int gridRatio)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        // create seperate lists for SpawnOptions by location type (CONRNER, WALL, CENTER, ANY)
        List<SpawnOption>[] optionLists = new List<SpawnOption>[4];
        for(int i = 0; i < 4; ++i)
        {
            optionLists[i] = new List<SpawnOption>();
        }

        
        foreach(SpawnOption so in spawnOptions)
        {
            optionLists[so.location].Add(so);
        }

        // center tile
        if ((length & 1) == 1 && (width & 1) == 1)
        {
            if (grid[length / 2, width / 2] == Room.EMPTY)
            {
                foreach (SpawnOption so in optionLists[SpawnOption.CENTER])
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < so.probability)
                    {
                        grid[length / 2, width / 2] = so.index;
                    }
                }
            }
        }

        // corner tiles. int i = 0 and i += length - 1 causes infinite loop if lenth == 1
        Vector2Int[] corners = new Vector2Int[4];
        corners[0] = new Vector2Int(0, 0);
        corners[1] = new Vector2Int(length - 1, 0);
        corners[2] = new Vector2Int(0, width - 1);
        corners[3] = new Vector2Int(length - 1, width - 1);

        foreach(Vector2Int v in corners)
        {
            int i = v.x;
            int j = v.y;

            if (grid[i, j] != Room.EMPTY)
                continue;


            foreach (SpawnOption so in optionLists[SpawnOption.CONRNER])
            {
                float rand = Random.Range(0f, 1f);
                if (rand < so.probability)
                {
                    grid[i, j] = so.index;
                }
            }
        }

        // wall tiles (inefficient but conside :P)
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                // skip any non-wall tiles
                if(i != 0 && i != length - 1)
                {
                    if (j != 0 && j != width - 1)
                    {
                        continue;
                    }
                }

                if (grid[i, j] != Room.EMPTY)
                    continue;

                foreach (SpawnOption so in optionLists[SpawnOption.WALL])
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < so.probability)
                    {
                        grid[i, j] = so.index;
                    }
                }
            }
        }

        // any tiles
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                if (grid[i, j] != Room.EMPTY)
                    continue;

                foreach (SpawnOption so in optionLists[SpawnOption.ANY])
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < so.probability)
                    {
                        grid[i, j] = so.index;
                    } else
                    {
                        Debug.Log("unlucky");
                    }
                }
            }
        }

        // take tiles on next smaller grid
        if (nextGrid != null)
        {
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (grid[i, j] != Room.EMPTY)
                        takeGridTiles(nextGrid, i * gridRatio, j * gridRatio, i * gridRatio + gridRatio - 1, j * gridRatio + gridRatio - 1);
                }
            }
        }
    }

    private void takeGridTiles(int[,] grid, int startX, int startY, int endX, int endY)
    {
        for(int i = startX; i <= endX; ++i)
        {
            for (int j = startY; j <= endY; ++j)
            {
                grid[i, j] = Room.TAKEN;
            }
        }
    }

    private void generateTileSpawns(int[,] grid, float tileSize, List<SpawnOption> spawnOptions)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        float startX = _floor.transform.position.x - _floor.transform.localScale.x / 2 + tileSize / 2;
        float startZ = _floor.transform.position.z - _floor.transform.localScale.z / 2 + tileSize / 2;

        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                int num = grid[i, j];
                if (num < 0)
                    continue;


                GameObject go = Instantiate(spawnOptions[num].gameObject);
                go.transform.position = new Vector3(startX + tileSize * i, go.transform.localScale.y/2, startZ + tileSize * j);
                go.transform.SetParent(this.transform);
            }
        }
    }

    public void SetGridVisible(bool b)
    {
        if(b)
        {
            _drawGridObjects = new List<GameObject>((int)(_largeSpawnGrid.GetLength(1)) * _largeSpawnGrid.GetLength(1));

            _drawGridObjects.AddRange(DrawFloorGrid(_xLargeSpawnGrid, XLARGE_GRID_SIZE, .01f, _xLargeTileMaterial));
            _drawGridObjects.AddRange(DrawFloorGrid(_largeSpawnGrid, LARGE_GRID_SIZE, .02f, _largeTileMaterial));
            _drawGridObjects.AddRange(DrawFloorGrid(_smallSpawnGrid, SMALL_GRID_SIZE, .03f, _smallTileMaterial));
        }
        else
        {
            if (_drawGridObjects == null)
                return;

            foreach(GameObject go in _drawGridObjects)
            {
                Destroy(go);
            }

            _drawGridObjects.Clear();

        }
    }

    private List<GameObject> DrawFloorGrid(int[,] grid, float tileSize, float height, Material mat)
    {

        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        List<GameObject> drawnTiles = new List<GameObject>(length * width);

        float startX = _floor.transform.position.x - _floor.transform.localScale.x / 2 + tileSize / 2;
        float startZ = _floor.transform.position.z - _floor.transform.localScale.z / 2 + tileSize / 2;

        //List<GameObject> gridCubes = new List<GameObject>(length * width);

        GameObject cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubePrefab.GetComponent<Renderer>().material = mat;
        cubePrefab.transform.localScale = new Vector3(tileSize - .1f, _floor.transform.localScale.y + height, tileSize - .1f);

        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                if (grid[i, j] == Room.TAKEN || grid[i, j] == Room.EMPTY)
                    continue;

                GameObject cube = Instantiate(cubePrefab);

                cube.transform.position = new Vector3(startX + tileSize * i, .01f, startZ + tileSize * j);
                cube.transform.SetParent(_floor.transform);
                drawnTiles.Add(cube);
            }
        }

        Destroy(cubePrefab);

        return drawnTiles;
    }
}