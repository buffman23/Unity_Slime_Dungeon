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

    public Vector3 size;

    public bool deadEnd = false;

    public Vector2Int gridPosition = Vector2Int.zero;

    public Vector2 doorwayDimensions = new Vector2(3f, 4f);

    public static float wallThickness = 1f;

    public const int NORTH_WALL = 0, EAST_WALL = 1, SOUTH_WALL = 2, WEST_WALL = 3;

    private GameObject[] _walls = new GameObject[4];

    private GameObject _floor;

    private GameObject _ceiling;

    private Material _roomMaterial;


    private void Awake()
    {
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

        // Temportary testing code
        //MakeDoorway(_walls[NORTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[EAST_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[SOUTH_WALL], Random.Range(0f, 1f));
        //MakeDoorway(_walls[WEST_WALL], Random.Range(0f, 1f));
    }
    // Start is called before the first frame update
    void Start()
    {

        
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
}