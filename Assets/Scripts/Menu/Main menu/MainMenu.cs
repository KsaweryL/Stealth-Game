using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //for currently available sounds
    public Slider soundfxVolumeSlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;

    public void QuitGame()
    {
        Debug.Log("Exited the game");
        Application.Quit();
    }

    private void Start()
    {
        soundfxVolumeSlider.value = StaticData.soundfxVolumeSliderValue;
        masterVolumeSlider.value = StaticData.masterVolumeSliderValue;
        musicVolumeSlider.value = StaticData.musicVolumeSliderValue;

        Debug.Log(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        StaticData.soundfxVolumeSliderValue = soundfxVolumeSlider.value;
        StaticData.masterVolumeSliderValue = masterVolumeSlider.value;
        StaticData.musicVolumeSliderValue = musicVolumeSlider.value;
    }

}
