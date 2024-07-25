using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class ChasingPlayer : MonoBehaviour
{

    public Transform player;
    private NavMeshAgent agent;
    public ThirdPersonMovement playerInfo;

    public bool canSeePlayer;
    public bool chasePlayer;
    public bool playerIsHidden;

    //get previous NPC destination
    public Vector3 previousDestination;

    [Header("Hiding in a cover")]
    public bool waitBeforePlayersCover;
    public int distanceToWaitBeforeCover;

    [Header("Time of chasing the player after he/she is lost")]
    public float timeToWaitAfterLosing;
    public int currentlyWaitingTimeToWaitAfterLosing;

    [Header("Connection with other components")]
    public NPCMovement npcMovement;
    public NPCFieldOfView npcFOV;

    [Header("Damage")]
    public float amountOfDamageToInflict;
    public float inflictDamageDistance;
    public float distnaceToTarget;
    public float timeBetweenHits;
    float currentTimeBetweenHits;
    public bool damageWasInflicted;

    [Header("chase")]
    public float chaseSpeedMultiplier;

    public void UpdateCanSeePlayerStatusChasingPlayer(bool canSeePlayerVariable)
    {
        canSeePlayer = canSeePlayerVariable;
        int currentlyDetectedTimeReversed = npcFOV.GetCurrentlyDetectedTimeReversedNPCFOV();
        if (canSeePlayer && currentlyDetectedTimeReversed == 0)
            chasePlayer = true;
    }

    public bool GetWaitBeforePlayersCoverChasingPlayer()
    {
        return waitBeforePlayersCover;
    }

    public void ChaseAfterPlayer()
    {

        //player is being chased

        //regarding losing the player from the sight
        //if player is being hunted but is in the bushes, the enemy should go nearby the last known area
        if (waitBeforePlayersCover == true)
        {
            float distnaceToTarget = Vector3.Distance(transform.position, player.position);

            //get out of this waiting state if the countdown reaches 0 or less
            if (currentlyWaitingTimeToWaitAfterLosing <= 0)
            {
                agent.isStopped = false;
                waitBeforePlayersCover = false;

                npcMovement.StopChasingPlayer();

                chasePlayer = false;
            }
            else if (playerIsHidden == false && canSeePlayer == true)
                waitBeforePlayersCover = false;
            else if (agent.isStopped == true && currentlyWaitingTimeToWaitAfterLosing > 0)
            {
                currentlyWaitingTimeToWaitAfterLosing -= 5;
            }
            //if the ~10 units iof distance is reached, wait in the last known spot for some time that depends on currentlyWaitingTimeToWaitAfterLosing
            else if (distnaceToTarget < distanceToWaitBeforeCover)
            {
                agent.isStopped = true;
            }
            
        }
        //if player is  seen and chasen after, set the currently waiting time to wait after losing to proper value;
        else if (canSeePlayer == true)
            currentlyWaitingTimeToWaitAfterLosing = (int)Math.Round(timeToWaitAfterLosing / 0.02);

        //if player is not seen and countdown is on
        else if (currentlyWaitingTimeToWaitAfterLosing > 0
            && canSeePlayer == false)
        {
            //if player is hidden, different scenario should play out (waitBeforePlayersCover)
            if (playerIsHidden == false)
                currentlyWaitingTimeToWaitAfterLosing--;
            else
                waitBeforePlayersCover = true;
        }

        //if player run away and is lost, also stop the chase, reset the waiting time
        else if (canSeePlayer == false
            && currentlyWaitingTimeToWaitAfterLosing <= 0)
        {

            npcMovement.StopChasingPlayer();

            chasePlayer = false;
        }

        //if waitBeforePlayersCover is not true, there is no reason for npc to stop
        if (!waitBeforePlayersCover)
            agent.isStopped = false;


        //if player is chasen after, isn't hidden and waitBeforePlayersCover is turned off, chase the player
        if (chasePlayer == true )
        {
            if (waitBeforePlayersCover == false && !playerIsHidden)
            {
                previousDestination = agent.destination;
                Debug.Log("Chasing the player");
                agent.isStopped = false;

                agent.destination = player.position;
                if(!damageWasInflicted)
                    agent.speed = playerInfo.GetSpeed() * chaseSpeedMultiplier;
                else
                    agent.speed = playerInfo.GetSpeed() * 0.3f * chaseSpeedMultiplier;
            }
        }


    }

    public void InflictDamage()
    {
        distnaceToTarget = Vector3.Distance(transform.position, player.position);

        if (distnaceToTarget > inflictDamageDistance)
            return;
        if (currentTimeBetweenHits <= 0)
        {
            playerInfo.TakeDamage(amountOfDamageToInflict);
            currentTimeBetweenHits = timeBetweenHits;
            damageWasInflicted = true;
        }
        else
            currentTimeBetweenHits--;

    }

    public void CheckIfPlayerHidden()
    {
        playerIsHidden = FindObjectOfType<DetectingPlayerInHidingSpot>().IsPlayerHidden();

    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcMovement = GetComponent<NPCMovement>();
        npcFOV = GetComponent<NPCFieldOfView>();
        playerInfo = player.GetComponent<ThirdPersonMovement>();

        canSeePlayer = false;
        waitBeforePlayersCover = false;

        currentlyWaitingTimeToWaitAfterLosing = 0;
        currentTimeBetweenHits = timeBetweenHits;

        damageWasInflicted = false;
    }

    private void FixedUpdate()
    {
        if (chasePlayer)
        {
            ChaseAfterPlayer();
            InflictDamage();
        }
        else
            damageWasInflicted = false;

        npcMovement.UpdateChasePlayerStatusNPCMovement(chasePlayer);
        npcFOV.UpdateChasePlayerStatusNPCFOV(chasePlayer);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerHidden();
    }
}
