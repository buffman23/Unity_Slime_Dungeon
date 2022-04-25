using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBarController : MonoBehaviour
{
    Image healthBar;
    int maxHealth = 100;
    public static HealthBarController instance;

    void Start()
    {
        if (HealthBarController.instance == null)
            HealthBarController.instance = this;
        else
            Destroy(this);
        initReferences();
    }

    private void initReferences()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    void Update()
    {
        
    }

    public void changeHealthBar(int newHealth)
    {
        // need to get current health

        // divide newHealth by 100 to get how much to decrease scale by on health bar

        // set X scale to the quotient
        healthBar.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 100 * newHealth / maxHealth);
        healthBar.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 18.7f);
    }
}
