using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public bool canPickup = true;
    private AudioClip _keyClip;
    private AudioSource _AS;

    // Start is called before the first frame update
    void Start()
    {
        _keyClip = Resources.Load<AudioClip>("Sounds/key");
        _AS = gameObject.AddComponent<AudioSource>();
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
            //Destroy(this.gameObject);
            StartCoroutine(KeyNoiseAndDie());
        }
    }

    private IEnumerator KeyNoiseAndDie()
    {
        GetComponent<MeshRenderer>().enabled = false;
        _AS.clip = _keyClip;
        _AS.Play();
        while (_AS.isPlaying)
        {
            yield return new WaitForSeconds(1f);
        }

        Destroy(this.gameObject);
    }
}
