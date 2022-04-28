using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToController : MonoBehaviour
{
    [SerializeField] GameObject secondPage;

    public void goToNext()
    {
        secondPage.SetActive(true);
    }

    public void goBack()
    {
        secondPage.SetActive(false);
    }

}
