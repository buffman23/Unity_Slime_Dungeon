using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MergeTiles : MonoBehaviour
{
    // Start is called before the first frame update

    public Tilemap tilemap;
    void Start()
    {
        tilemap.ClearAllTiles();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tilemap.HasTile(new Vector3Int(0, 0, 0)));
        Debug.Log(tilemap.HasTile(new Vector3Int(1, 0, 0)));
        Debug.Log(tilemap.HasTile(new Vector3Int(0, 1, 0)));
        Debug.Log(tilemap.HasTile(new Vector3Int(0, 0, 1)));
    }
}
