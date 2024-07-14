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

    //detecting a player
    public bool playerInSight;
    public float sightRange;

    public LayerMask whatIsPlayer;

    void PlayerDetection()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        Debug.Log(playerInSight);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pointNr = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //agent.destination = player.position;

        actualPosition = agent.transform.position;

        //update only when the NPC reaches a point
        if (actualPosition == agent.destination)
        {
            if (pointNr != numberOfPoints - 1)
                pointNr++;
            else if (pointNr == numberOfPoints - 1)
                pointNr = 0;
        }

        agent.destination = PathPoints[pointNr].transform.position;

        PlayerDetection();
    }
}
