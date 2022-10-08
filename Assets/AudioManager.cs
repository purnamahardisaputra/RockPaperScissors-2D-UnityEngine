
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider SFXSlider;
    public Slider BGMSlider;
    public AudioMixer mixer;
    public bool muted;
    // public AudioMixerGroup BGM;
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
        Debug.Log("SFX Volume" + " " + (value + 80) + "%");
    }

    public void BGMVolume(float value)
    {
        value = value * 80 - 80;
        mixer.SetFloat("BGM_Vol", value);
        Debug.Log("BGM Volume" + " " + (value + 80) + "%");
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
