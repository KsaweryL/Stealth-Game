using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class ChasingPlayer : MonoBehaviour
{

    public Transform player;
    private NavMeshAgent agent;

    public bool wasPlayerSpotted;
    public bool chasePlayer;
    public bool playerIsHidden;

    //waiting before the chase
    public float timeToWait;
    int currentlyWaitingTime;

    //get previous NPC destination
    public Vector3 previousDestination;

    public void UpdatePlayerStatus(bool isPlayerSpotted)
    {
        wasPlayerSpotted = isPlayerSpotted;
        if(wasPlayerSpotted)
            chasePlayer = true;
    }

    public void ChaseAfterPlayer()
    {
        //if player is spotted, wait a second and start chasing him/her
        if (currentlyWaitingTime == 0)
        {
            previousDestination = agent.destination;
            agent.destination = player.position;
        }
        else
            currentlyWaitingTime--;

        //if player is hidden and is not spotted, give up the chase nad reset the waiting time
        if (playerIsHidden && !wasPlayerSpotted)
        {
            
            FindAnyObjectByType<NPC2Movement>().StopChasingPlayer();

            currentlyWaitingTime = (int)Math.Round(timeToWait / 0.02);
            chasePlayer = false;
        }

    }

    public void CheckIfPlayerHidden()
    {
        playerIsHidden = FindObjectOfType<DetectingPlayerInHidingSpot>().IsPlayerHidden();

    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentlyWaitingTime = (int)Math.Round(timeToWait / 0.02);
    }

    private void FixedUpdate()
    {
        if(chasePlayer)
            ChaseAfterPlayer();

       
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerHidden();
    }
}
