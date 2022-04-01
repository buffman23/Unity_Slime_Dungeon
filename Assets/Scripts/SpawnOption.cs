/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: Data Structure to hold information about a room spawn tile
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class SpawnOption : MonoBehaviour
{
    public SpawnOption(SpawnOption so) : base()
    {
        this.probability = so.probability;
        this.location = so.location;
        this.rotation = so.rotation;
        this.maxPerRoom = so.maxPerRoom;
    }

    public const int ANY = 0, WALL = 1, CORNER = 2, CENTER = 3;
    public const int NO_ROTATION = 0, SQUARE_ROTATION = 1, ANY_ROTATION = 2;

    public float probability; // probability that this GameObject will be put on a tile
    public int location; // location in room this GameObject will be places. Not x,y coordinates, but tile type.
    public int rotation;
    public int maxPerRoom; // max spawns per room(
    public bool includeInTilePool = true;
}

