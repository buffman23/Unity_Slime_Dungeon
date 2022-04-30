using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public float difficulty;
    public Color playerColor;

    private void Awake()
    {
        if(instance == null)
        {
            GameController.instance = this;
            DontDestroyOnLoad(this);
        } 
        else
        {
            Destroy(this.gameObject);
        }

        difficulty = 1;
        playerColor = Color.black;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
