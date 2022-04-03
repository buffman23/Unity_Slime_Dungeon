using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasTrap : MonoBehaviour
{
    private Dictionary<GameObject, Rigidbody> _touching = new Dictionary<GameObject, Rigidbody>();
    private HashSet<Rigidbody> _rigidBodies = new HashSet<Rigidbody>();
    private Dictionary<GameObject, PlayerController> _touchingPlayer = new Dictionary<GameObject, PlayerController>();
    private HashSet<PlayerController> _playerControllers = new HashSet<PlayerController>();

    public static Vector3 upForce = new Vector3(0f, 50f, 0f);

    private static Vector3 _up = new Vector3(0f, 1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in _rigidBodies)
        {
            rb.AddForce(upForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        foreach (PlayerController pc in _playerControllers)
        {
            Vector3 upForce = new Vector3(0f, 50f, 0f);
            pc.AddImpact(_up, 50f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        Debug.Log("Trigger hit " + go.name);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        
        if(rb == null)
        {
            Transform parentTrans = go.transform.parent;
            if (parentTrans)
            {
                rb = parentTrans.GetComponent<Rigidbody>();
            }
        }

        if (rb != null)
        {
            Debug.Log("Added" + go.name);
            _touching[go] = rb;
            _rigidBodies.Add(rb);
            return;
        }


        PlayerController pc = go.GetComponent<PlayerController>();

        if (pc == null)
        {
            Transform parentTrans = go.transform.parent;
            if (parentTrans)
            {
                pc = parentTrans.GetComponent<PlayerController>();
            }
        }

        if (pc != null)
        {
            Debug.Log("Added player controller");
            _touchingPlayer[go] = pc;
            _playerControllers.Add(pc);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;

        try
        {
            Rigidbody rb = _touching[go];
            if (rb != null)
            {
                _touching.Remove(go);
                _rigidBodies.Remove(rb);
                return;
            }
        }catch(KeyNotFoundException ex)
        {

        }

        try { 
         PlayerController pc = _touchingPlayer[go];
            if (pc != null)
            {
                _touchingPlayer.Remove(go);
                _playerControllers.Remove(pc);
                return;
            }
        }
        catch (KeyNotFoundException ex)
        {

        }
    }
}
