using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class NPC2Movement : MonoBehaviour
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
    public int timeToWait;
    int currentlyWaitingTime;

    //detecting a player
    public bool wasPlayerDetected;
    public bool chasePlayer;

    public void StopChasingPlayer()
    {
        chasePlayer = false;
    }
    public void UpdatePlayerStatus(bool isPlayerSpotted)
    {
        wasPlayerDetected = isPlayerSpotted;
        if(wasPlayerDetected)
            chasePlayer = true;
    }

    void UpdateWaitingTime()
    {
        if (currentlyWaitingTime == 0)
        {
            agent.destination = PathPoints[pointNr].transform.position;
            currentlyWaitingTime = timeToWait;
        }
        else
            currentlyWaitingTime--;
    }

    void NPCPatrolling()
    {
        //agent.destination = player.position;

        actualPosition = agent.transform.position;

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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pointNr = 0;

        currentlyWaitingTime = 0;
    }

    private void FixedUpdate()
    {
        //Patrol only if player is not detected
        //once the player is spotted, chase him for a certain period of time,
        //even if player is not directly detected 
        if (!chasePlayer)
            NPCPatrolling();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
