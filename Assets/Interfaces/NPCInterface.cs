using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NPCInterface
{
    //animation

    //general mechanics
    //Chasing Player
    void UpdateCanSeePlayerStatusChasingPlayer(bool canSeePlayerVariable);
    //NPC FOV
    void UpdateChasePlayerStatusNPCFOV(bool chasePlayerVariable);
    void UpdateHiddenStatusNPCFOV(bool Hidden);
    int GetCurrentlyDetectedTimeReversedNPCFOV();
    float GetTimeToDetectNPCFOV();
    int GetCurrentlyDetectedTimeNPCFOV();
    bool GetCanSeePlayerNPCFOV();
    //NPC Movement
    void UpdateChasePlayerStatusNPCMovement(bool chasePlayerVariable);

    //UI


}
