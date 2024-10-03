using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Slider soundfxVolumeSlider;

    private void Start()
    {
        soundfxVolumeSlider.value = soundfxVolumeSlider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        float volume = soundfxVolumeSlider.value/soundfxVolumeSlider.maxValue;

        SoundFXManager.instance.ChangeVolume(volume, "SoundFXVolume");
    }
}
