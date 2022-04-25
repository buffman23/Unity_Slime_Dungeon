using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool swinging;
    private int damage = 34;

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
        GameObject go = other.gameObject;

        if (go == null)
            return;

        Slime enemy = go.transform.GetComponentInChildren<Slime>();

        
        if (enemy != null)
        {
            enemy.damageSlime(damage);
        }
        else
        {
            Transform parentTrans = go.transform.parent;

            if (parentTrans == null)
                return;

            enemy = go.transform.parent.GetComponent<Slime>();
            if (enemy != null && swinging)
            {
                enemy.damageSlime(damage);
            }
        }
    }
}
