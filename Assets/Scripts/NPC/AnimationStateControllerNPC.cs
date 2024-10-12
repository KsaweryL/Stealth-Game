using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AnimationStateControllerNPC : MonoBehaviour
{
    //animation
    Animator animator;
    bool isNPCMovingForward;

    private NavMeshAgent NPC;
    public GameObject[] PathPoints;
    private NPCMovement NPCmovement;

    public int numberOfPoints;

    //condition needed for sprinting
    public bool runningEnabled;
    public bool wait;

    public void UpdateRunningStatusAnimationSCNPC(bool runningEnabledVar)
    {
        runningEnabled = runningEnabledVar;
    }

    public void UpdateWaitAnimationSCNPC(bool waitVar)
    {
        wait = waitVar;
    }

    // Start is called before the first frame update
    void Start()
    {
        NPC = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        NPCmovement = GetComponent<NPCMovement>();

        PathPoints = NPCmovement.GetPathPoints();
        numberOfPoints = PathPoints.Length;
    }

    // Update is called once per frame
    void Update()
    {
        isNPCMovingForward = animator.GetBool("isNPCMovingForward");
        animator.SetBool("isNPCMovingForward", true);
        PathPoints = NPCmovement.GetPathPoints();

        if (wait == true)
            animator.SetBool("isNPCMovingForward", false);
        else
            animator.SetBool("isNPCMovingForward", true);

        //find if any of the points were reached
        for (int i = 0; i < numberOfPoints; i++) {
            if (NPC.transform.position.x == PathPoints[i].transform.position.x &&
                NPC.transform.position.z == PathPoints[i].transform.position.z &&
                Math.Ceiling(NPC.transform.position.y) == Math.Ceiling(PathPoints[i].transform.position.y))
            {
                animator.SetBool("isNPCMovingForward", false);
                //Debug.Log(animator.GetBool("isNPCMovingForward"));
            }
        }
        
        if(runningEnabled)
            animator.SetBool("isNPCRunning", true);
        else
            animator.SetBool("isNPCRunning", false);

        

    }
}
