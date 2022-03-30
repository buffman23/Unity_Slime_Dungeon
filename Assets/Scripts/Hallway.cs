/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: This class is a hallway connecting two rooms
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;


class Hallway : Room
{
    private void Start()
    {
        _ceiling.transform.position = new Vector3(_ceiling.transform.position.x, 
            _floor.transform.position.y + doorwayDimensions.y * this.transform.localScale.y + _ceiling.transform.localScale.y -.001f,
            _ceiling.transform.position.z);

        // fix crack in floor connecting to room
        
        if(_floor.transform.localScale.x > _floor.transform.localScale.z)
        {
            _floor.transform.localScale = new Vector3(_floor.transform.localScale.x + .01f, _floor.transform.localScale.y - .001f, _floor.transform.localScale.z);
        }
        else
        {
            _floor.transform.localScale = new Vector3(_floor.transform.localScale.x, _floor.transform.localScale.y - .001f, _floor.transform.localScale.z + .01f);
        }
    }
    public void Trapify()
    {

    }
}



