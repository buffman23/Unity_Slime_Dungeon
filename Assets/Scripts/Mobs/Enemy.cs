using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject key;

    private Rigidbody _keyRB;

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

    public virtual void kill()
    {
        if(key != null)
        {
            key.transform.SetParent(null);
            key.AddComponent<Rigidbody>();
        }

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    //protected virtual void Update()
    //{
        
    //}
}
