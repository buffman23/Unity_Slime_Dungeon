/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: Data Structure to hold information about a room spawn tile
 * 
 */

using UnityEngine;

public class SpawnOption : MonoBehaviour
{
    public const int ANY = 0, WALL = 1, CONRNER = 2, CENTER = 3;
    public const int NO_ROTATION = 0, SQUARE_ROTATION = 1, ANY_ROTATION = 2;

    public float probability; // probability that this GameObject will be put on a tile
    public int location; // location in room this GameObject will be places. Not x,y coordinates, but tile type.
    public int rotation;
}

