using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected static PlayerController _playerController;

    protected virtual void Start()
    {
        InitReferences();
    }

    private void InitReferences()
    {
        if (_playerController == null)
            _playerController = PlayerController.instance;
    }

    // Update is called once per frame
    //protected virtual void Update()
    //{
        
    //}
}
