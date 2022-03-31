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

    public const int NE_CORNER = 0, SE_CORNER = 1, SW_CORNER = 2, NW_CORNER = 3;

    private static GameObject _lightPrefab, _doorPrefab;

    private GameObject[] _walls = new GameObject[4];

    protected GameObject _floor;

    protected GameObject[,] _floorTiles;

    protected GameObject _ceiling;

    private static Material _roomMaterial, _smallTileMaterial, _largeTileMaterial, _xLargeTileMaterial;

    private List<GameObject> _spawnObjects;

    private int[,] _smallSpawnGrid, _largeSpawnGrid, _xLargeSpawnGrid;

    private List<GameObject> _drawGridObjects;


    private List<GameObject> _doors = new List<GameObject>(1);



    public const int SMALL_GRID_SIZE = 1,  LARGE_GRID_SIZE = 8,  XLARGE_GRID_SIZE = 16;

    public const int EMPTY = -1, TAKEN = -2, FOO = -3;

    private void Awake()
    {
        initReferences();

        _roomMaterial = Resources.Load<Material>("materials/Room");

        Vector2 scale;

        if (this is Hallway)
        {
            _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _floor.transform.SetParent(transform);
            _floor.transform.localPosition = new Vector3(0, 0, 0);
            _floor.transform.localScale = new Vector3(size.x, wallThickness, size.z);
            _floor.name = transform.gameObject.name + "_floor";
            _floor.GetComponent<Renderer>().material = _roomMaterial;
            scale = _floor.GetComponent<Renderer>().material.mainTextureScale;
            _floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * _floor.transform.lossyScale.x,
                scale.y * _floor.transform.lossyScale.z);
            _floor.layer = LayerMask.NameToLayer("Ground");
        }
        else
        {
            _floor = new GameObject();
            _floor.transform.SetParent(transform);
            _floor.transform.localScale = new Vector3(size.x, wallThickness, size.z);
            _floor.transform.localPosition = new Vector3(0, 0, 0);
            _floor.name = transform.gameObject.name + "_floor";

            GameObject floorTilePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //floorTilePrefab.transform.SetParent(transform);
            floorTilePrefab.transform.localPosition = new Vector3(0, 0, 0);
            floorTilePrefab.transform.localScale = new Vector3(LARGE_GRID_SIZE, wallThickness, LARGE_GRID_SIZE);
            floorTilePrefab.name = transform.gameObject.name + "_floor";
            floorTilePrefab.GetComponent<Renderer>().material = _roomMaterial;
            floorTilePrefab.layer = LayerMask.NameToLayer("Ground");

            floorTilePrefab.GetComponent<Renderer>().material = _roomMaterial;
            scale = floorTilePrefab.GetComponent<Renderer>().material.mainTextureScale;
            floorTilePrefab.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * floorTilePrefab.transform.lossyScale.x,
                scale.y * floorTilePrefab.transform.lossyScale.z);
            floorTilePrefab.layer = LayerMask.NameToLayer("Ground");

            float startX = _floor.transform.position.x - size.x / 2 + LARGE_GRID_SIZE / 2;
            float startZ = _floor.transform.position.z - size.z / 2 + LARGE_GRID_SIZE / 2;

            _floorTiles = new GameObject[(int)size.x / LARGE_GRID_SIZE, (int)size.z / LARGE_GRID_SIZE];
            for (int i = 0; i < size.x / LARGE_GRID_SIZE; ++i)
            {
                for (int j = 0; j < size.z / LARGE_GRID_SIZE; ++j)
                {
                    GameObject floorTile = GameObject.Instantiate(floorTilePrefab);
                    floorTile.transform.position = new Vector3(startX + LARGE_GRID_SIZE * i, 0f, startZ + LARGE_GRID_SIZE * j);
                    floorTile.transform.SetParent(_floor.transform);
                    floorTile.name = transform.gameObject.name + "_floorTile(" + i + ',' + j + ')';

                    _floorTiles[i, j] = floorTile;
                }
            }

            Destroy(floorTilePrefab);
        }

        

        

        _ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _ceiling.transform.SetParent(transform);
        _ceiling.transform.localPosition = new Vector3(0, size.y + wallThickness, 0);
        _ceiling.transform.localScale = new Vector3(size.x, wallThickness, size.z);
        _ceiling.name = transform.gameObject.name + "_ceiling";
        _ceiling.GetComponent<Renderer>().material = _roomMaterial;
        scale = _ceiling.GetComponent<Renderer>().material.mainTextureScale;
        _ceiling.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * _ceiling.transform.lossyScale.x,
            scale.y * _ceiling.transform.lossyScale.z);
        Destroy(_ceiling.GetComponent<BoxCollider>());

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
            scale = w.GetComponent<Renderer>().material.mainTextureScale;
            w.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * w.transform.lossyScale.z, scale.y * w.transform.lossyScale.y);
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

        _drawGridObjects = new List<GameObject>((int)(_largeSpawnGrid.GetLength(1)) * _largeSpawnGrid.GetLength(1));

        initPresetSpawnOptions();

        // Temportary testing code
        //MakeDoorway(_walls[NORTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[EAST_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[SOUTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[WEST_WALL], Random.Range(0f, 1f));
    }
    // Start is called before the first frame update
    private void initReferences()
    {
        if(_smallTileMaterial == null)
            _smallTileMaterial = Resources.Load<Material>("Materials/SmallTile");

        if (_largeTileMaterial == null)
            _largeTileMaterial = Resources.Load<Material>("Materials/LargeTile");

        if (_xLargeTileMaterial == null)
            _xLargeTileMaterial = Resources.Load<Material>("Materials/XLargeTile");

        if(_lightPrefab == null)
            _lightPrefab = Resources.Load<GameObject>("Prefabs/SpawnOptions/WallLight");

        if(_doorPrefab == null)
            _doorPrefab = Resources.Load<GameObject>("Prefabs/Door");
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTiles()
    {
        if (_xLargeSpawnGrid.GetLength(0) != 0 && _xLargeSpawnGrid.GetLength(1) != 0)
            GenerateTileNums(_xLargeSpawnGrid, _largeSpawnGrid, xLargeSpawnOptions, XLARGE_GRID_SIZE / LARGE_GRID_SIZE);
        GenerateTileNums(_largeSpawnGrid, null, largeSpawnOptions, LARGE_GRID_SIZE / SMALL_GRID_SIZE);
        GenerateTileNums(_smallSpawnGrid, null, smallSpawnOptions, 1);

        GenerateTiles(_xLargeSpawnGrid, XLARGE_GRID_SIZE, xLargeSpawnOptions);
        GenerateTiles(_largeSpawnGrid, LARGE_GRID_SIZE, largeSpawnOptions);
        GenerateTiles(_smallSpawnGrid, SMALL_GRID_SIZE, smallSpawnOptions);
    }

    private void initPresetSpawnOptions()
    {
        float roomArea = size.x * size.z;
        int lightCount = 1;
        if(roomArea > 529)
        {
            lightCount = 2;
        }

        // wall tiles
        int length = _smallSpawnGrid.GetLength(0);
        int width = _smallSpawnGrid.GetLength(1);
        List<Vector2Int> wallsPos = new List<Vector2Int>(length * width);
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                // skip any non-wall tiles
                if (i != 0 && i != length - 1)
                {
                    if (j != 0 && j != width - 1)
                    {
                        continue;
                    }
                }

                wallsPos.Add(new Vector2Int(i, j));
            }
        }

        float startX = _floor.transform.position.x - _floor.transform.lossyScale.x / 2 + SMALL_GRID_SIZE / 2f;
        float startZ = _floor.transform.position.z - _floor.transform.localScale.z / 2 + SMALL_GRID_SIZE / 2f;

        for (int i = 0; i < lightCount; ++i)
        {
            Vector2Int randPos = wallsPos[(int)Random.Range(0, wallsPos.Count)];
            int randX = randPos.x;
            int randY = randPos.y;
            if (_smallSpawnGrid[randX, randY] == Room.TAKEN)
            {
                --i;
                continue;
            }
            _smallSpawnGrid[randX, randY] = Room.TAKEN;

            GameObject lamp = Instantiate(_lightPrefab);
            GameObject soFloor = lamp.transform.Find("Floor").gameObject;
            float yPos = _floor.transform.position.y + _floor.transform.lossyScale.y / 2 - soFloor.transform.lossyScale.y / 2;
            lamp.transform.position = new Vector3(startX + SMALL_GRID_SIZE * randX, yPos, startZ + SMALL_GRID_SIZE * randY);

            float rotation = getWallTileRotation(_smallSpawnGrid, randX, randY);
            lamp.transform.eulerAngles = new Vector3(lamp.transform.rotation.x, rotation, lamp.transform.rotation.z);
            lamp.transform.SetParent(transform);

            Destroy(soFloor);
            Destroy(lamp.transform.Find("Wall1").gameObject);
        }
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


    public void MakeDoorway(Vector2 side, float position, bool makeDoor)
    {
        int x = 0, y = 0;
        int wall_const = NORTH_WALL;

        if (side.x == 1)
        {
            wall_const = NORTH_WALL;

            x = _largeSpawnGrid.GetLength(0) - 1;
            y = _largeSpawnGrid.GetLength(1) / 2;
        }
        else if (side.x == -1)
        {
            wall_const = SOUTH_WALL;

            x = 0;
            y = _largeSpawnGrid.GetLength(1) / 2;
        }
        else if (side.y == 1)
        {
            wall_const = WEST_WALL;

            x = _largeSpawnGrid.GetLength(0) / 2;
            y = _largeSpawnGrid.GetLength(1) - 1;
        }
        else if (side.y == -1)
        {
            wall_const = EAST_WALL;

            x = _largeSpawnGrid.GetLength(0) / 2;
            y = 0;
            
        }

        _largeSpawnGrid[x, y] = Room.TAKEN;

        MakeDoorway(_walls[wall_const], position, makeDoor);


    }

    public void MakeDoorway(int side, float position, bool makeDoor)
    {
        MakeDoorway(_walls[side], position, makeDoor);
    }

    /*
     * Authors: Ryan
     * Desc: Adds a doorway to a wall given a relative wall position.
     * Params: wall - The wall to add a doorway to.
     *         position - Relative wall position [0-1].
     */
    private void MakeDoorway(GameObject wall, float position, bool makeDoor)
    {
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.layer = LayerMask.NameToLayer("Ground");
        float topHeight = wall.transform.localScale.y - doorwayDimensions.y;
        top.transform.localScale = new Vector3(wallThickness, topHeight, wall.transform.localScale.z);
        top.transform.rotation = wall.transform.rotation;
        top.transform.SetParent(wall.transform);
        top.transform.localPosition = new Vector3(0, .5f - top.transform.localScale.y/2f, 0);
        top.name = wall.name + "_TopWall";
        top.GetComponent<Renderer>().material = _roomMaterial;

        Vector2 scale = top.GetComponent<Renderer>().material.mainTextureScale;
        top.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * top.transform.lossyScale.z, scale.y * top.transform.lossyScale.y);


        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left.transform.localScale = new Vector3(wallThickness, wall.transform.localScale.y - topHeight, 
            wall.transform.localScale.z * position - doorwayDimensions.x/2f);
        left.transform.rotation = wall.transform.rotation;
        left.transform.SetParent(wall.transform);
        left.transform.localPosition = new Vector3(0, -.5f + left.transform.localScale.y/2f,
            .5f - left.transform.localScale.z/2f);
        left.name = wall.name + "_LeftWall";
        left.GetComponent<Renderer>().material = _roomMaterial;

        scale = left.GetComponent<Renderer>().material.mainTextureScale;
        left.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * left.transform.lossyScale.z, scale.y * left.transform.lossyScale.y);

        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right.transform.localScale = new Vector3(wallThickness, wall.transform.localScale.y - topHeight,
            wall.transform.localScale.z * (1 - position) - doorwayDimensions.x / 2f);
        right.transform.rotation = wall.transform.rotation;
        right.transform.SetParent(wall.transform);
        right.transform.localPosition = new Vector3(0, -.5f + right.transform.localScale.y / 2f,
            -.5f + right.transform.localScale.z / 2f);
        right.name = wall.name + "_RightWall";
        right.GetComponent<Renderer>().material = _roomMaterial;

        scale = right.GetComponent<Renderer>().material.mainTextureScale;
        right.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * right.transform.lossyScale.z, scale.y * right.transform.lossyScale.y);

        if (makeDoor)
        {
            GameObject door = Instantiate(_doorPrefab);
            //door.transform.position = new Vector3(wall.transform.position.x,
            //    wall.transform.position.y - wall.transform.lossyScale.y/2f + door.transform.lossyScale.y/2f, 
            //    wall.transform.position.z);

            Transform doorChild = door.transform.Find("Cube");
            door.transform.rotation = wall.transform.rotation;
            door.transform.Rotate(0f, 90f, 0f);
            door.transform.SetParent(wall.transform);
            door.transform.localPosition = new Vector3(0f, -.5f + (doorChild.localScale.y * door.transform.localScale.y)/2f, 0f);
            door.name = "Door";
        }

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

    private void GenerateTileNums(int[,] grid, int[,] nextGrid, List<SpawnOption> spawnOptions, int gridRatio)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        // create seperate lists for SpawnOptions by location type (CONRNER, WALL, CENTER, ANY)
        List<SpawnOption>[] optionLists = new List<SpawnOption>[4];
        for (int i = 0; i < 4; ++i)
        {
            optionLists[i] = new List<SpawnOption>();
        }

        // associate SpawnOption withs its position in spawnOptions list
        Dictionary<SpawnOption, int> soIndexDict = new Dictionary<SpawnOption, int>(spawnOptions.Count * 2);
        Dictionary<SpawnOption, int> soMaxDict = new Dictionary<SpawnOption, int>(spawnOptions.Count * 2);

        for (int i = 0; i < spawnOptions.Count; ++i)
        {
            SpawnOption so = spawnOptions[i];
            soIndexDict.Add(so, i);
            soMaxDict.Add(so, so.maxPerRoom);
            optionLists[so.location].Add(so);
        }



        // center tile
        if ((length & 1) == 1 && (width & 1) == 1)
        {
            if (grid[length / 2, width / 2] == Room.EMPTY)
            {

                SpawnOption so = pickSpawnOption(optionLists[SpawnOption.CENTER], soMaxDict);

                if (so != null)
                {
                    grid[length / 2, width / 2] = soIndexDict[so];
                }
            }
        }

        // corner tiles. int i = 0 and i += length - 1 causes infinite loop if lenth == 1
        Vector2Int[] corners = new Vector2Int[4];
        corners[0] = new Vector2Int(0, 0);
        corners[1] = new Vector2Int(length - 1, 0);
        corners[2] = new Vector2Int(0, width - 1);
        corners[3] = new Vector2Int(length - 1, width - 1);

        for (List<Vector2Int> cornersPos = new List<Vector2Int>(corners); cornersPos.Count > 0;)
        {
            int randIdx = Random.Range(0, cornersPos.Count);
            Vector2Int v = cornersPos[randIdx];

            if (grid[v.x, v.y] != Room.EMPTY)
            {
                cornersPos.RemoveAt(randIdx);
                continue;
            }


            SpawnOption so = pickSpawnOption(optionLists[SpawnOption.CORNER], soMaxDict);

            if (so != null)
            {
                grid[v.x, v.y] = soIndexDict[so];
            }

            cornersPos.RemoveAt(randIdx);
        }

        // wall tiles
        List<Vector2Int> wallsPos = new List<Vector2Int>(grid.GetLength(0) * grid.GetLength(1));
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                // skip any non-wall tiles
                if (i != 0 && i != length - 1)
                {
                    if (j != 0 && j != width - 1)
                    {
                        continue;
                    }
                }

                wallsPos.Add(new Vector2Int(i, j));
            }
        }

        while (wallsPos.Count > 0)
        {
            int randIdx = Random.Range(0, wallsPos.Count);
            Vector2Int v = wallsPos[randIdx];

            if (grid[v.x, v.y] != Room.EMPTY)
            {
                wallsPos.RemoveAt(randIdx);
                continue;
            }

            SpawnOption so = pickSpawnOption(optionLists[SpawnOption.WALL], soMaxDict);

            if (so != null)
            {
                grid[v.x, v.y] = soIndexDict[so];
            }

            wallsPos.RemoveAt(randIdx);
        }

        // any tiles
        List<Vector2Int> anyPos = new List<Vector2Int>(grid.GetLength(0) * grid.GetLength(1));
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                anyPos.Add(new Vector2Int(i, j));
            }
        }

        while (anyPos.Count > 0)
        {
            int randIdx = Random.Range(0, anyPos.Count);
            Vector2Int v = anyPos[randIdx];

            if (grid[v.x, v.y] != Room.EMPTY)
            {
                anyPos.RemoveAt(randIdx);
                continue;
            }

            SpawnOption so = pickSpawnOption(optionLists[SpawnOption.ANY], soMaxDict);

            if (so != null)
            {
                grid[v.x, v.y] = soIndexDict[so];
            }

            anyPos.RemoveAt(randIdx);
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

    private SpawnOption pickSpawnOption(List<SpawnOption> spawnOptions, Dictionary<SpawnOption, int> maxSpawnDict)
    {
        List<SpawnOption> spawnOptionsDup = new List<SpawnOption>(spawnOptions);

        while(spawnOptionsDup.Count > 0)
        {
            int randIdx = (int)Random.Range(0, spawnOptionsDup.Count);
            SpawnOption so = spawnOptionsDup[randIdx];

            float rand = Random.Range(0f, 1f);
            if (rand < so.probability)
            {
                if (--maxSpawnDict[so] == 0)
                {
                    spawnOptions.Remove(so);
                }
                return so;
            }

            spawnOptionsDup.RemoveAt(randIdx);
        }

        return null;
    }

    private void GenerateTiles(int[,] grid, float tileSize, List<SpawnOption> spawnOptions)
    {

        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        List<GameObject> drawnTiles = new List<GameObject>(length * width);

        float startX = _floor.transform.position.x - _floor.transform.localScale.x / 2 + tileSize / 2;
        float startZ = _floor.transform.position.z - _floor.transform.localScale.z / 2 + tileSize / 2;

        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                if (grid[i, j] == Room.TAKEN || grid[i, j] == Room.EMPTY)
                    continue;

                SpawnOption soPrefab = spawnOptions[grid[i, j]];

                SpawnOption so = Instantiate(soPrefab);

                GameObject soFloor = getChild(so, "Floor");
                float yPos = _floor.transform.position.y + _floor.transform.localScale.y / 2 - soFloor.transform.localScale.y/2;
                so.transform.position = new Vector3(startX + tileSize * i, yPos, startZ + tileSize * j);

                if (soFloor.tag.Equals("Delete"))
                {
                    Destroy(_floorTiles[i, j]);
                }

                if (so.rotation == SpawnOption.ANY_ROTATION) {
                    float randRotation = UnityEngine.Random.Range(0f, 359f);
                    so.transform.eulerAngles = new Vector3(so.transform.rotation.x, randRotation, so.transform.rotation.z);
                } else if(so.rotation == SpawnOption.SQUARE_ROTATION)
                {
                    float randRotation = UnityEngine.Random.Range(0f, 359f);
                    randRotation = randRotation - randRotation % 90;
                    so.transform.eulerAngles = new Vector3(so.transform.rotation.x, randRotation, so.transform.rotation.z);
                }
                else
                {
                    if (so.location == SpawnOption.WALL)
                    {
                        float rotation = getWallTileRotation(grid, i, j);
                        so.transform.eulerAngles = new Vector3(so.transform.rotation.x, rotation, so.transform.rotation.z);
                    } else if (so.location == SpawnOption.CORNER)
                    {
                        float rotation = getCornerTileRotation(grid, i, j);
                        so.transform.eulerAngles = new Vector3(so.transform.rotation.x, rotation, so.transform.rotation.z);
                    }
                    else
                    {
                        //float randRotation = UnityEngine.Random.Range(0f, 359f);
                        //so.transform.eulerAngles = new Vector3(so.transform.rotation.x, randRotation, so.transform.rotation.z);
                    }
                }

                StripTemplateObjets(so);

                so.transform.parent = this.transform;
                //so.GetComponent<SpawnOption>().enabled = false;

                //cube.transform.position = new Vector3(startX + tileSize * i, .01f, startZ + tileSize * j);
                //cube.transform.SetParent(_floor.transform);
                //drawnTiles.Add(cube);
            }
        }
    }

    private float getWallTileRotation(int[,] grid, int x, int y)
    {
        if(x == 0)
            return 270f;

        if (x == grid.GetLength(0) - 1)
            return 90f;

        if (y == 0)
            return 180f;

        if (y == grid.GetLength(1) - 1)
            return 0f;

        return -10;
    }

    private float getCornerTileRotation(int[,] grid, int x, int y)
    {
        if (x == 0)
        {
            if (y == 0)
                return 180f;

            return 270f;
        }
        if (y == 0)
            return 90f;

        return 0f;

             
    }

    private GameObject getChild(SpawnOption so, string childName)
    {
        Transform[] children = so.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Equals(childName))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void StripTemplateObjets(SpawnOption so)
    {
        Transform[] children = so.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Equals("Floor") || child.name.Equals("Wall1") || child.name.Equals("Wall2"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SetGridVisible(bool b)
    {
        foreach (GameObject go in _drawGridObjects)
        {
            Destroy(go);
        }

        _drawGridObjects.Clear();

        if (b)
        {
            _drawGridObjects.AddRange(DrawFloorGrid(_xLargeSpawnGrid, XLARGE_GRID_SIZE, .01f, _xLargeTileMaterial));
            _drawGridObjects.AddRange(DrawFloorGrid(_largeSpawnGrid, LARGE_GRID_SIZE, .02f, _largeTileMaterial));
            _drawGridObjects.AddRange(DrawFloorGrid(_smallSpawnGrid, SMALL_GRID_SIZE, .03f, _smallTileMaterial));
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