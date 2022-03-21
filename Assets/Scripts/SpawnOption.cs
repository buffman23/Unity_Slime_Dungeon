/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: Data Structure to hold information about a room spawn tile
 * 
 */

using UnityEngine;

public class SpawnOption
{
    public const int CONRNER = 0, WALL = 1, CENTER = 2, ANY = 3;

    public GameObject gameObject; // the GameObject that will be placed on a room tile
    public float probability; // probability that this GameObject will be put on a tile
    public int location; // location in room this GameObject will be places. Not x,y coordinates, but tile type.
    public int index; // index of this instance in the list it is in. this needs to be refactored.

    public SpawnOption(GameObject gameObject, float probability, int location)
    {
        this.gameObject = gameObject;
        this.probability = probability;
        this.location = location;
    }
}

