using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool swinging;

    private BoxCollider _bladeCollider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void InitReferences()
    {
        _bladeCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Blade hit " + other.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Swinging:" + swinging);
    }
}
