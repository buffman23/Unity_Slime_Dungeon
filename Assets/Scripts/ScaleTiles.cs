using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTiles : MonoBehaviour
{
    public Material material;
    public GameObject[] gos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject go in gos)
        {
            Material mat = go.GetComponent<Renderer>().material;
            Vector2 scale = mat.mainTextureScale;
            mat.mainTextureScale = new Vector2(scale.x * go.transform.lossyScale.x, scale.y * go.transform.lossyScale.z);
            Vector2 foo = new Vector2(go.transform.lossyScale.x, go.transform.lossyScale.z);
           
        }
        Destroy(this);
    }
}
