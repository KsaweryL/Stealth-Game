using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NPCMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public GameObject[] PathPoints;

    public int numberOfPoints;
    public float speed;

    private Vector3 actualPosition;
    private int pointNr;

    public LayerMask whatIsPlayer;

    //waiting a number of frames
    public int timeToWaitWhenPatrolling;
    int currentlyWaitingTime;

    //detecting a player
    public bool chasePlayer;

    [Header("Connection with other components")]
    public NPCFieldOfView NPCFOV;
    public ChasingPlayer chasingPlayer;

    public GameObject[] GetPathPoints()
    {
        return PathPoints;
    }
    public void StopChasingPlayer()
    {
        chasePlayer = false;
        agent.destination = PathPoints[pointNr].transform.position;
    }
    public void UpdateChasePlayerStatusNPCMovement(bool chasePlayerVariable)
    {
        chasePlayer = chasePlayerVariable;
    }
    void UpdateWaitingTime()
    {
        if (currentlyWaitingTime == 0)
        {
            agent.destination = PathPoints[pointNr].transform.position;
            currentlyWaitingTime = (int)Math.Round(timeToWaitWhenPatrolling / 0.02);
        }
        else
            currentlyWaitingTime--;
    }

    void NPCPatrolling()
    {

        actualPosition = agent.transform.position;
        agent.speed = speed;

        //update only when the NPC reaches a point
        if (actualPosition == agent.destination && currentlyWaitingTime == 0)
        {
            if (pointNr != numberOfPoints - 1)
                pointNr++;
            else if (pointNr == numberOfPoints - 1)
                pointNr = 0;

        }

        UpdateWaitingTime();
    }

    public bool IsPlayerBeingDetected()
    {
        int currentlyDetectedTime = NPCFOV.GetCurrentlyDetectedTimeNPCFOV();
        if (currentlyDetectedTime != 0)
            return true;
        else 
            return false;
    }

    public bool WaitIfDetectingPlayer()
    {
        if (IsPlayerBeingDetected())
        {
            agent.isStopped = true;
            return true;
        }
        //also tied with waitBeforePlayersCover, therefore agent.isStopped needs to be additionally controlled
        //here
        else if (!chasingPlayer.GetWaitBeforePlayersCoverChasingPlayer())
        {
            agent.isStopped = false;
            return false;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        NPCFOV = GetComponent<NPCFieldOfView>();
        chasingPlayer = GetComponent<ChasingPlayer>();
        pointNr = 0;

        currentlyWaitingTime = 0;
        numberOfPoints = PathPoints.Length;
        player = FindObjectOfType<ThirdPersonMovement>().transform;
    }

    private void FixedUpdate()
    {

        //Patrol only if player is not already detected
        //once the player is spotted, chase him for a certain period of time,
        //even if player is not directly detected 

        if (!chasePlayer && !WaitIfDetectingPlayer())
            NPCPatrolling();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
