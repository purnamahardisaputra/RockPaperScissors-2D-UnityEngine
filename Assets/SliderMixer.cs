using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderMixer : MonoBehaviour
{
    public Slider SFXSlider;
    public AudioMixer mixer;

    private void Start()
    {
        float db;
        mixer.GetFloat("SFX_Vol", out db);
        SFXSlider.value = (db + 80) / 80;
    }
}
