using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTemplateGenerator : MonoBehaviour
{
    public const float TILE_THICKNESS = .25f;
    public const float WALL_HEIGHT = 5f;

    private Material _material;

    private void Start()
    {
        InitReferences();

        
        GenerateFloorTile(Room.XLARGE_GRID_SIZE);
        GenerateWallTile(Room.XLARGE_GRID_SIZE);
        GenerateCornerTile(Room.XLARGE_GRID_SIZE);

        GenerateFloorTile(Room.LARGE_GRID_SIZE);
        GenerateWallTile(Room.LARGE_GRID_SIZE);
        GenerateCornerTile(Room.LARGE_GRID_SIZE);

        GenerateFloorTile(Room.SMALL_GRID_SIZE);
        //GenerateWallTile(Room.SMALL_GRID_SIZE);
        //GenerateCornerTile(Room.SMALL_GRID_SIZE);
    }

    private void InitReferences()
    {
        _material = Resources.Load<Material>("Materials/SmallTile");
    }

    public GameObject GenerateFloorTile(int tileSize)
    {
        GameObject template = new GameObject();
        template.tag = "TileTemplate";
        template.name = "FloorTileTemplate";

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.GetComponent<MeshRenderer>().material = _material;

        floor.transform.localScale = new Vector3(tileSize, TILE_THICKNESS, tileSize);

        floor.transform.SetParent(template.transform);

        return template;
    }

    public GameObject GenerateWallTile(int tileSize)
    {
        GameObject template = GenerateFloorTile(tileSize);
        template.name = "WallTileTemplate";

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall1";
        wall.GetComponent<MeshRenderer>().material = _material;

        wall.transform.localScale = new Vector3(tileSize, WALL_HEIGHT, TILE_THICKNESS);
        wall.transform.position = new Vector3(0, WALL_HEIGHT/2 + TILE_THICKNESS/2, tileSize / 2 + TILE_THICKNESS/2);

        wall.transform.SetParent(template.transform);

        return template;
    }

    public GameObject GenerateCornerTile(int tileSize)
    {
        GameObject template = GenerateWallTile(tileSize);
        template.name = "CornerTileTemplate";

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall2";
        wall.GetComponent<MeshRenderer>().material = _material;
        wall.transform.localScale = new Vector3(TILE_THICKNESS, 5f, tileSize);
        wall.transform.position = new Vector3(tileSize / 2 + TILE_THICKNESS/2, WALL_HEIGHT / 2 + TILE_THICKNESS/2, 0);

        wall.transform.SetParent(template.transform);

        return template;
    }
}
