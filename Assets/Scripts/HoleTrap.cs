using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrap : MonoBehaviour
{
    private BoxCollider _collisionBox;
    private PlayerController playerController;
    private Transform _respawnTrans;

    // Start is called before the first frame update
    void Start()
    {
        InitReferences();


    }

    private void InitReferences()
    {
        _collisionBox = GetComponent<BoxCollider>();
        _respawnTrans = transform.Find("Respawn");
        if (playerController == null)
            playerController = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        
        GameObject go = collider.gameObject;
        Debug.Log("Trigger entered: " + go.name);
        if (go.gameObject.name == "Player")
            playerController.damage(40);
        Vector3 respawnPosition = _respawnTrans.position + new Vector3(0f, go.transform.lossyScale.y / 2f, 0f);

        CharacterController cc = go.GetComponent<CharacterController>();
        if (cc != null)
        {
            bool wasEnabled = cc.enabled;
            cc.enabled = false;
            go.transform.position = respawnPosition;
            cc.enabled = wasEnabled;
        }
        else 
        {
            go.transform.position = respawnPosition;
        }
    }
}
