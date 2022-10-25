
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

    private void Awake()
    {
        float db;

        if (mixer.GetFloat("SFX_Vol", out db))
            SFXSlider.value = (db + 80) / 80;

        if (mixer.GetFloat("BGM_Vol", out db))
            BGMSlider.value = (db + 80) / 80;

        if (PlayerPrefs.GetInt("Mute") == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false; ;
        }
    }

    public void SFXVolume(float value)
    {
        value = value * 80 - 80;
        mixer.SetFloat("SFX_Vol", value);
        var sum = ((value / 80) * 100) + 100;
        Debug.Log("SFX Volume " + sum + "%");
        PlayerPrefs.SetFloat("SFX_Vol", value);
        PlayerPrefs.Save();
    }

    public void BGMVolume(float value)
    {
        value = value * 80 - 80;
        mixer.SetFloat("BGM_Vol", value);
        var sum = ((value / 80) * 100) + 100;
        Debug.Log("BGM Volume " + sum + "%");
        PlayerPrefs.SetFloat("BGM_Vol", value);
        PlayerPrefs.Save();
    }

    public void MuteToggle()
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetInt("Mute", 1);
            AudioListener.pause = true;
            Debug.Log("Audio Muted");
        }
        else
        {
            PlayerPrefs.SetInt("Mute", 0);
            AudioListener.pause = false;
            Debug.Log("Audio Unmuted");
        }
        PlayerPrefs.Save();
    }

}
