using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCFieldOfView : MonoBehaviour
{
    [Header("Adjustable variables")]
    public float radius;
    [Range(0, 360)]
    public float angle;

    [Header("Detecting the player")]
    public float timeToDetect;
    int currentlyDetectedTimeReversed;
    public int currentlyDetectedTime;

    [Header("Other")]
    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public LayerMask notFullBarrier;

    public bool canSeePlayer;
    public bool playerIsHidden;

    public bool chasePlayer;

    [Header("Connection with other components")]
    public ChasingPlayer chasingPlayer;

    [Header("Distance from NPC to Player")]
    public float distnaceToTarget;

    public void UpdateChasePlayerStatusNPCFOV(bool chasePlayerVariable)
    {
        chasePlayer = chasePlayerVariable;
    }
    public void UpdateHiddenStatusNPCFOV()
    {
        playerIsHidden = FindObjectOfType<DetectingPlayerInHidingSpot>().IsPlayerHidden(); ;
    }

    public int GetCurrentlyDetectedTimeReversedNPCFOV()
    {
        return currentlyDetectedTimeReversed;
    }

    public float GetTimeToDetectNPCFOV()
    {
        return timeToDetect;
    }
    public int GetCurrentlyDetectedTimeNPCFOV()
    {
        return currentlyDetectedTime;
    }

    public bool GetCanSeePlayerNPCFOV()
    {
        return canSeePlayer;
    }

    private void FieldOfViewCheck()
    {

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            //since we can opnly detect 1 plr, we will set target to the 0th member of the array
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            distnaceToTarget = Vector3.Distance(transform.position, target.position);

            //if the player is within our sight
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {

                //our raycast would differ depending on whether player is crouching or not
                //bool isPlayerSneakingBehindCover;

                //returns false if the ray intersects with the collider (any obstruction) or if distance to target is 0
                if (!Physics.Raycast(transform.position, directionToTarget, distnaceToTarget, obstructionMask) || distnaceToTarget < 2)
                {
                    //if it's a "not full barrier" and player is crouching, we can't see the player
                    if (Physics.Raycast(transform.position, directionToTarget, distnaceToTarget, notFullBarrier) && FindObjectOfType<ThirdPersonMovement>().GetIsSnkeaing())
                    {
                        canSeePlayer = false;
                    }
                    else
                        canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            //if it's outside our sight and is not diorectly onto us
            else if (distnaceToTarget >= 2)
                canSeePlayer = false;
        }
        else if (canSeePlayer == true && distnaceToTarget >= 2)
            canSeePlayer = false;


    }

    public void UpdateDetectedTime()
    {
        //update the detection variables only when player is being seen
        if (canSeePlayer)
        {
            if (currentlyDetectedTimeReversed == 0)
            {

            }
            else
            {
                currentlyDetectedTimeReversed--;
                currentlyDetectedTime++;
            }
        }
        else if (!chasePlayer)
        {
                currentlyDetectedTimeReversed = (int)Math.Round(timeToDetect / 0.02);
                currentlyDetectedTime = 0;

        }
        else if (chasePlayer)
        {
            currentlyDetectedTime = (int)Math.Round(timeToDetect / 0.02);
            currentlyDetectedTimeReversed = 0;

        }
        else if (currentlyDetectedTime > 0)
        {
            currentlyDetectedTime--;
            if (currentlyDetectedTimeReversed != (int)Math.Round(timeToDetect / 0.02)) 
                currentlyDetectedTimeReversed++;
        }
    }
    private void UpdatePlayerStatus()
    {
        //update the status when player is detected
        UpdateDetectedTime();
        chasingPlayer.UpdateCanSeePlayerStatusChasingPlayer(canSeePlayer);
        UpdateHiddenStatusNPCFOV();


    }

    //looks for the player
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            //check fov only when player is not hidden
            if (!playerIsHidden)
                FieldOfViewCheck();
            UpdatePlayerStatus();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentlyDetectedTimeReversed = (int)Math.Round(timeToDetect / 0.02);
        currentlyDetectedTime = 0;
        chasePlayer = false;

        //connection with other components
        chasingPlayer = GetComponent<ChasingPlayer>();

        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        //for debugging
        if(!FindObjectOfType<ThirdPersonMovement>().GetIsSnkeaing())
            Debug.DrawLine(transform.position + new Vector3(0, 1f, 0),  FindObjectOfType<ThirdPersonMovement>().transform.position + new Vector3(0, 0.5f, 0), Color.green);
        else
        {
            Debug.DrawLine(transform.position + new Vector3(0, 1f, 0), FindObjectOfType<ThirdPersonMovement>().transform.position, Color.green);
        }
    }
}
