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
    public float timeToDetectWhenRunning;
    public float timeToDetectWhenCrouching;
    public float timeToDetectWhenCrouchingRunning;
    int currentlyDetectedTimeReversed;
    public int currentlyDetectedTime;
    private float currentlySetTimeToDetect;
    public Transform player;

    [Header("Other")]
    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask[] obstructionMasks;
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
        if(chasePlayer == false)
            canSeePlayer = false;

    }
    public void UpdateHiddenStatusNPCFOV()
    {
        playerIsHidden = player.GetComponent<DetectingPlayerInHidingSpot>().IsPlayerHidden(); ;
    }

    public int GetCurrentlyDetectedTimeReversedNPCFOV()
    {
        return currentlyDetectedTimeReversed;
    }

    public float GetTimeToDetectNPCFOV()
    {
        return currentlySetTimeToDetect;
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

                //for every obstructionMask
                for (int obstructionMask = 0; obstructionMask < obstructionMasks.Length; obstructionMask++)
                {
                    //returns false if the ray intersects with the collider (any obstruction) or if distance to target is 0
                    if (!Physics.Raycast(transform.position, directionToTarget, distnaceToTarget, obstructionMasks[obstructionMask]) || distnaceToTarget < 2)
                    {
                        //if it's a "not full barrier" and player is crouching, we can't see the player
                        if (Physics.Raycast(transform.position, directionToTarget, distnaceToTarget, notFullBarrier) && player.GetComponent<ThirdPersonMovement>().GetIsSneaking())
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
        //don't update the time when the pause menu is on
        if (!GetComponentInParent<Game>().GetIsPauseMenuOn())
        {
            //change the time to detect whenever player is not seen
            if (!canSeePlayer)
            {
                bool isSprintEnabledVariable = player.GetComponent<ThirdPersonMovement>().IsSprintEnabled();
                bool isSneakingVariable = player.GetComponent<ThirdPersonMovement>().GetIsSneaking();
                //if player is sprinting and is sneaking
                if (isSprintEnabledVariable && isSneakingVariable)
                    currentlySetTimeToDetect = timeToDetectWhenCrouchingRunning;
                //when player is only sneaking
                else if (!isSprintEnabledVariable && isSneakingVariable)
                    currentlySetTimeToDetect = timeToDetectWhenCrouching;
                else if (isSprintEnabledVariable && !isSneakingVariable)
                    currentlySetTimeToDetect = timeToDetectWhenRunning;
                else if (!isSprintEnabledVariable && !isSneakingVariable)
                    currentlySetTimeToDetect = timeToDetect;
            }

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
                currentlyDetectedTimeReversed = (int)Math.Round(currentlySetTimeToDetect / 0.02);
                currentlyDetectedTime = 0;

            }
            else if (chasePlayer)
            {
                currentlyDetectedTime = (int)Math.Round(currentlySetTimeToDetect / 0.02);
                currentlyDetectedTimeReversed = 0;

            }
            else if (currentlyDetectedTime > 0)
            {
                currentlyDetectedTime--;
                if (currentlyDetectedTimeReversed != (int)Math.Round(currentlySetTimeToDetect / 0.02))
                    currentlyDetectedTimeReversed++;
            }
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

            if(GetComponentInParent<Game>().GetIsTrainingOn())
                GetComponentInParent<Game>().GetPlayer().GetComponent<MLPlayerAgent>().PlayerDetectionCheck(canSeePlayer);
        }
    }

    private void InitiateTimeToDetect()
    {
        //if these values are not manually set, give the normal ones
        if(timeToDetect <= 0)
        {
            timeToDetect = 0.2f;
        }

        if (timeToDetectWhenRunning <= 0)
        {
            timeToDetectWhenRunning = timeToDetect/2;
        }

        if (timeToDetectWhenCrouching <= 0)
        {
            timeToDetectWhenCrouching = timeToDetect * 2;
        }

        if (timeToDetectWhenCrouchingRunning <= 0)
        {
            timeToDetectWhenCrouchingRunning = timeToDetect * 1.5f;
        }

        currentlySetTimeToDetect = timeToDetect;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitiateTimeToDetect();
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentlyDetectedTimeReversed = (int)Math.Round(currentlySetTimeToDetect / 0.02);
        currentlyDetectedTime = 0;
        chasePlayer = false;

        //connection with other components
        player = GetComponentInParent<Game>().GetPlayer().transform;
        chasingPlayer = GetComponent<ChasingPlayer>();

        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        //for debugging
        //if(!player.GetComponent<ThirdPersonMovement>().GetIsSneaking())
        //    Debug.DrawLine(transform.position + new Vector3(0, 1f, 0),  player.GetComponent<ThirdPersonMovement>().transform.position + new Vector3(0, 0.5f, 0), Color.green);
        //else
        //{
        //    Debug.DrawLine(transform.position + new Vector3(0, 1f, 0), player.GetComponent<ThirdPersonMovement>().transform.position, Color.green);
        //}
    }
}
