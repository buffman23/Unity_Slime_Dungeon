using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public bool canPickup = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!canPickup)
            return;

        PlayerController pc;

        if((pc = collider.gameObject.GetComponent<PlayerController>()) == null)
        {
            return;
        }

        if (!pc.hasKey())
        {
            pc.setHasKey(true);
            Destroy(this.gameObject);
        }
    }
}
