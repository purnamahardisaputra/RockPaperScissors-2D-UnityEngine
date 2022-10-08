
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider SFXSlider;
    public Slider BGMSlider;
    public AudioMixer mixer;
    public Toggle toggle;
    private bool muted;

    private void Awake()
    {
        if (AudioListener.pause == true)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }

    private void Start()
    {

        float db;

        if (mixer.GetFloat("SFX_Vol", out db))
            SFXSlider.value = (db + 80) / 80;

        if (mixer.GetFloat("BGM_Vol", out db))
            BGMSlider.value = (db + 80) / 80;
    }
    public void SFXVolume(float value)
    {
        value = value * 80 - 80;
        mixer.SetFloat("SFX_Vol", value);
        var sum = ((value / 80) * 100) + 100;
        Debug.Log("SFX Volume " + sum + "%");
    }

    public void BGMVolume(float value)
    {
        value = value * 80 - 80;
        mixer.SetFloat("BGM_Vol", value);
        var sum = ((value / 80) * 100) + 100;
        Debug.Log("BGM Volume " + sum + "%");
    }

    public void MuteToggle()
    {
        if (muted == false)
        {
            muted = true;
            AudioListener.pause = true;
            Debug.Log("Audio Muted");
        }
        else
        {
            muted = false;
            AudioListener.pause = false;
            Debug.Log("Audio Unmuted");
        }
    }

}
