using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundFXManager : MonoBehaviour
{
    //this will be set up as singleton
    public static SoundFXManager instance;

    //we need to mix some sounds
    [SerializeField] private AudioMixer audioMixer;

    //for currently available sounds
    public Slider soundfxVolumeSlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void ApplyJumpingSound(bool jump, AudioSource audioSource, bool isCharacterGrounded)
    {
        if (!(GetComponentInParent<Game>().GetIsTrainingOn()) && jump && isCharacterGrounded)
        {
            audioMixer.SetFloat("PlayerWalkingOrRunningSoundFXVolume", -50);
            audioMixer.SetFloat("PlayerJumpingSoundFXVolume", 0);

            audioSource.Play();
        }
        else
            audioMixer.SetFloat("PlayerWalkingOrRunningSoundFXVolume", 0);



    }

    public bool ApplyWalkingSound(float horizontal, float vertical, bool sneakingButton, bool sprint, float ySpeed, AudioSource audioSource)
    {
        if ((!(GetComponentInParent<Game>().GetIsTrainingOn()) && ySpeed == -1.0f)) {
            //play sound fx
            if ((horizontal != 0f || vertical != 0) && (!sprint && !sneakingButton))
            {
                audioSource.enabled = true;
            }
            else
                audioSource.enabled = false;

            return audioSource.enabled;
        }

        return false;
    }

    public bool ApplyRunningSound(float horizontal, float vertical, bool sprint, bool sneakingButton, float ySpeed, AudioSource audioSource)
    {
        if ((!(GetComponentInParent<Game>().GetIsTrainingOn()) && ySpeed == -1.0f) && (horizontal != 0f || vertical != 0) && sprint && !sneakingButton)
        {
            audioSource.enabled = true;
        }
        else
            audioSource.enabled = false;

        return audioSource.enabled;
    }

    public bool ApplySneakingSound(float horizontal, float vertical, bool sprint, bool sneakingButton, float ySpeed, AudioSource audioSource)
    {
        if ((!(GetComponentInParent<Game>().GetIsTrainingOn()) && ySpeed == -1.0f) && (horizontal != 0f || vertical != 0) && !sprint && sneakingButton)
        {
            audioSource.enabled = true;
        }
        else
            audioSource.enabled = false;

        return audioSource.enabled;
    }

    public bool ApplySneakingRunningSound(float horizontal, float vertical, bool sprint, bool sneakingButton, float ySpeed, AudioSource audioSource)
    {
        if ((!(GetComponentInParent<Game>().GetIsTrainingOn()) && ySpeed == -1.0f) && (horizontal != 0f || vertical != 0) && sprint && sneakingButton)
        {
            audioSource.enabled = true;
        }
        else
            audioSource.enabled = false;

        return audioSource.enabled;
    }

    public void ApplyBeingDetectedSound(float beingDetectedSoundMultiplier, AudioSource audioSource, bool volumeChange)
    {
        if (beingDetectedSoundMultiplier != 0)
        {
            if (volumeChange)
            {
                audioSource.volume = beingDetectedSoundMultiplier * 0.4f;
            }
            else
                audioSource.volume = 1;
            audioSource.pitch = 1 + beingDetectedSoundMultiplier * 1.3f;
            audioSource.enabled = true;
        }
        else
            audioSource.enabled= false;
    }

    public void ChangeVolume(float volume, string audioMixerParameter)
    {
        //0% is -80
        //1% is -10
        //100% is 20
        float audioMixerVolume = 0;
        if (volume != 0)
            audioMixerVolume = volume * 30 - 10;
        else
            audioMixerVolume = -80;


        //it has value from -80 to 0
        audioMixer.SetFloat(audioMixerParameter, audioMixerVolume);
    }

    private void Start()
    {
        soundfxVolumeSlider.value = soundfxVolumeSlider.maxValue * 0.4f;
        masterVolumeSlider.value = soundfxVolumeSlider.maxValue * 0.4f;
        musicVolumeSlider.value = soundfxVolumeSlider.maxValue * 0.4f;
    }

    // Update is called once per frame
    void Update()
    {

        ChangeVolume(soundfxVolumeSlider.value, "SoundFXVolume");
        ChangeVolume(musicVolumeSlider.value, "MusicVolume");
        ChangeVolume(masterVolumeSlider.value, "MasterVolume");
    }
}
