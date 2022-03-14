﻿/*
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

    private GameObject[] _walls = new GameObject[4];

    private GameObject _floor;

    private GameObject _ceiling;

    private float _wallThickness = 1f;

    private const int NORTH_WALL = 0, EAST_WALL = 1, SOUTH_WALL = 2, WEST_WALL = 3;

    private Material _roomMaterial;

    private Vector2 _doorwayDimensions = new Vector2(3f, 4f);


    // Start is called before the first frame update
    void Start()
    {

        _roomMaterial = Resources.Load<Material>("materials/Room");

        _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _floor.transform.SetParent(transform);
        _floor.transform.localPosition = new Vector3(0, 0, 0);
        _floor.transform.localScale = new Vector3(size.x, _wallThickness, size.z);
        _floor.name = transform.gameObject.name + "__floor";
        _floor.GetComponent<Renderer>().material = _roomMaterial;
        _floor.layer = LayerMask.NameToLayer("Ground");

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(size.x / 2f + _wallThickness / 2f, size.y / 2f + _wallThickness / 2f, 0);
        wall.transform.localScale = new Vector3(_wallThickness, size.y, size.z);
        wall.name = transform.gameObject.name + "_NorthWall";
        _walls[NORTH_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(-size.x / 2f - _wallThickness / 2f, size.y / 2f + _wallThickness / 2f, 0);
        wall.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(_wallThickness, size.y, size.z);
        wall.name = transform.gameObject.name + "_SouthWall";
        _walls[SOUTH_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(0, size.y / 2f + _wallThickness / 2f, -size.z / 2f - _wallThickness / 2f);
        wall.transform.Rotate(new Vector3(0f, 90f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(_wallThickness, size.y, size.x);
        wall.name = transform.gameObject.name + "EastWall";
        _walls[EAST_WALL] = wall;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = new Vector3(0, size.y / 2f + _wallThickness / 2f, size.z / 2f + _wallThickness / 2f);
        wall.transform.Rotate(new Vector3(0f, 270f, 0f), Space.Self);
        wall.transform.localScale = new Vector3(_wallThickness, size.y, size.x);
        wall.name = transform.gameObject.name + "_WestWall";
        _walls[WEST_WALL] = wall;

        foreach(GameObject w in _walls){
            w.GetComponent<Renderer>().material = _roomMaterial;
            w.layer = LayerMask.NameToLayer("Ground");
        }

        // Temportary testing code
        MakeDoorway(_walls[NORTH_WALL], .25f);
        MakeDoorway(_walls[EAST_WALL], .5f);
        MakeDoorway(_walls[SOUTH_WALL], .1f);
        MakeDoorway(_walls[WEST_WALL], .67f);
    }

    // Update is called once per frame
    void Update()
    {

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
        top.transform.localScale = new Vector3(_wallThickness, wall.transform.localScale.y - _doorwayDimensions.y, wall.transform.localScale.z);
        top.transform.rotation = wall.transform.rotation;
        top.transform.SetParent(wall.transform);
        top.transform.localPosition = new Vector3(0, .5f - top.transform.localScale.y/2f, 0);
        top.name = wall.name + "_TopWall";
        top.GetComponent<Renderer>().material = _roomMaterial;

        Vector2 scale = top.GetComponent<Renderer>().material.mainTextureScale;
        top.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * top.transform.localScale.z, scale.y * top.transform.localScale.y);


        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left.transform.localScale = new Vector3(_wallThickness, wall.transform.localScale.y - top.transform.localScale.x, 
            wall.transform.localScale.z * position - _doorwayDimensions.x/2f);
        left.transform.rotation = wall.transform.rotation;
        left.transform.SetParent(wall.transform);
        left.transform.localPosition = new Vector3(0, -.5f + left.transform.localScale.y/2f,
            .5f - left.transform.localScale.z/2f);
        left.name = wall.name + "_LeftWall";
        left.GetComponent<Renderer>().material = _roomMaterial;

        scale = left.GetComponent<Renderer>().material.mainTextureScale;
        left.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x * left.transform.localScale.z, scale.y * left.transform.localScale.y);

        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right.transform.localScale = new Vector3(_wallThickness, wall.transform.localScale.y - top.transform.localScale.x,
            wall.transform.localScale.z * (1 - position) - _doorwayDimensions.x / 2f);
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
}
