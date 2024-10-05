using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeartBeatAudioSource : MonoBehaviour
{
    public float timeToDetect;
    public float currentlyDetectedTime;
    float beingDetectedSoundMultiplier;


    private void Start()
    {
        
    }

    private void Update()
    {
        //get currently detected time from NPC
        NPCMovement[] npcs = GetComponentInParent<Game>().GetNPCmovements();
        NPCFieldOfView chosenNPC = npcs[0].GetComponent<NPCFieldOfView>();

        foreach (NPCMovement npc in npcs) {
            if (npc.GetComponent<NPCFieldOfView>().GetCurrentlyDetectedTimeNPCFOV() > chosenNPC.GetCurrentlyDetectedTimeNPCFOV())
                chosenNPC = npc.GetComponent<NPCFieldOfView>();
        }

        timeToDetect = (int)Math.Round(chosenNPC.GetTimeToDetectNPCFOV() / 0.02);
        currentlyDetectedTime = chosenNPC.GetCurrentlyDetectedTimeNPCFOV();

        //if player stopped being chased, start turning down the music
        if (beingDetectedSoundMultiplier != 0 && currentlyDetectedTime == 0)
        {
            if (beingDetectedSoundMultiplier > 0.5f)
            {
                beingDetectedSoundMultiplier -= 0.002f;
                SoundFXManager.instance.ApplyBeingDetectedSound(beingDetectedSoundMultiplier, GetComponentInParent<ThirdPersonMovement>().GetComponentInChildren<BeingDetectedMusic>().GetComponent<AudioSource>(), true);
                SoundFXManager.instance.ApplyBeingDetectedSound(beingDetectedSoundMultiplier, GetComponent<AudioSource>(), true);
            }
            else
                beingDetectedSoundMultiplier = currentlyDetectedTime / timeToDetect;
        }
        //else, music starts playing
        else
        {
            beingDetectedSoundMultiplier = currentlyDetectedTime / timeToDetect;
            SoundFXManager.instance.ApplyBeingDetectedSound(beingDetectedSoundMultiplier, GetComponent<AudioSource>(), false);
            SoundFXManager.instance.ApplyBeingDetectedSound(beingDetectedSoundMultiplier * 0.3f, GetComponentInParent<ThirdPersonMovement>().GetComponentInChildren<BeingDetectedMusic>().GetComponent<AudioSource>(), true);
        }
    }
}
