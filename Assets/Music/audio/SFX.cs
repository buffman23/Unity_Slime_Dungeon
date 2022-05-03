using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class SFXPlaying : MonoBehaviour
{
    public AudioSource SwordSwing;

    public void PlaySwordSWing()
    {
        SwordSwing.Play();
    }
}
