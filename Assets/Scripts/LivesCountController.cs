using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LivesCountController : MonoBehaviour
{
    Text livesCount;
    public static LivesCountController instance;
    
    // Start is called before the first frame update
    void Start()
    {
        initReferences();
        if (LivesCountController.instance == null)
            LivesCountController.instance = this;
        else
            Destroy(this);
    }
    void initReferences()
    {
        livesCount = GameObject.Find("LivesCount").GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void decrementNumOfLives(int numOfLives)
    {
        livesCount.text = numOfLives.ToString();
    }
}
