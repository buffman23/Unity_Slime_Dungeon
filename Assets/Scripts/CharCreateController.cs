using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharCreateController : MonoBehaviour
{
    private Slider _hueSlider;
    private SkinnedMeshRenderer _SMR;

    void Start()
    {
        InitReferences();

        _hueSlider.onValueChanged.AddListener(HueSliderChanged);
    }

    void InitReferences()
    {
        _hueSlider = GameObject.Find("HueSlider").GetComponent<Slider>();
        _SMR = GameObject.Find("PlayerCube").GetComponent<SkinnedMeshRenderer>();
    }

    void HueSliderChanged(float newValue)
    {
        if (newValue == 0)
        {
            _SMR.material.color = Color.white;
        }
        else
        {
            _SMR.material.color = Color.HSVToRGB(newValue, .5f, .5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
