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
            Rigidbody rb;
            rb = enemy.gameObject.GetComponent<Rigidbody>();
            enemy.damageSlime(damage);
            rb.AddForce(enemy.gameObject.transform.up * 5f, ForceMode.VelocityChange);
            rb.AddForce(PlayerController.instance.transform.forward * 5f, ForceMode.VelocityChange);
        }
        else
        {
            Transform parentTrans = go.transform.parent;

            if (parentTrans == null)
                return;

            enemy = go.transform.parent.GetComponent<Slime>();

            if (enemy != null && swinging)
            {
                Rigidbody rb;
                rb = enemy.gameObject.GetComponent<Rigidbody>();
                enemy.damageSlime(damage);
                rb.AddForce(enemy.gameObject.transform.up * 5f, ForceMode.VelocityChange);
                rb.AddForce(PlayerController.instance.transform.forward * 5f, ForceMode.VelocityChange);
            }
        }
    }
}
