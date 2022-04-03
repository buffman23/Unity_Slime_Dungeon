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
        Debug.Log("Blade hit " + other.gameObject.transform.root.name);
        GameObject go = other.gameObject;

        if (go == null)
            return;

        Enemy enemy = go.transform.GetComponentInChildren<Enemy>();

        
        if (enemy != null)
        {
            enemy.kill();
        }
        else
        {
            Transform parentTrans = go.transform.parent;

            if (parentTrans == null)
                return;

            enemy = go.transform.parent.GetComponent<Enemy>();
            if (enemy != null && swinging)
            {
                enemy.kill();
            }
        }
    }
}
