using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Events;

public class MLPlayerAgent : Agent
{
    [SerializeField] private Transform diamondTransform;

    //I need to invoke things to all NPCS
    public NPCMovement[] NPCmovement;

    public CharacterController controller;

    public Vector3 startingPlayerPosition;
    public List<Vector3> diamondsPositions;

    Diamond[] allDiamonds;
    int collectedDiamonds;
    float moveSpeed;
    float usedTime;

    void Start()
    {
        //needs to be dynamic
        diamondsPositions = new List<Vector3>();

        //for the player
        startingPlayerPosition = new Vector3 (-0.11f, -2f, -42.56f);
        collectedDiamonds = 0;
        controller = GetComponent<CharacterController>();
        usedTime = Time.deltaTime;

        allDiamonds = FindObjectsOfType<Diamond>();
        NPCmovement = FindObjectsOfType<NPCMovement>();

        //important! I have an absolute posiotion here, not a local one as in the inspecotr
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            diamondsPositions.Add(allDiamonds[diamond].transform.localPosition);
        
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New Episode");
        //we need to disbale controller to avoid collisions
        controller.enabled = false;
        transform.localPosition = startingPlayerPosition;
        controller.enabled = true;

        //resetting properties
        GetComponent<ThirdPersonMovement>().ResetCurrentHealth();
        GetComponent<PlayerInventory>().ResetProperties();

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCmovement[npc].ResetPropertiesCall();

        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            allDiamonds[diamond].ResetProperties();




    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //adding all of the diamond positions
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            sensor.AddObservation(diamondsPositions[diamond]);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveY = actions.ContinuousActions[2];

        moveSpeed  = FindObjectOfType<ThirdPersonMovement>().GetSpeed();
        Vector3 moveDir = new Vector3(moveX, moveY, moveZ);
        if(controller)
            controller.Move(moveDir.normalized * moveSpeed * usedTime);

    }

    //Heuristics are mental shortcuts for solving problems in a quick way that delivers a result that is sufficient enough to be useful given time constraints
    //Implement this function to provide custom decision making logic or to support manual control of an agent using keyboard, mouse, game controller input, or a script.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        Vector3 moveDir = GetComponent<ThirdPersonMovement>().GetMoveDir();
        continuousActions[0] = moveDir.x;      //x
        continuousActions[1] = moveDir.z;        //z
        continuousActions[2] = moveDir.y;       //y

    }

    public void DiamondWasCollected()
    {
        collectedDiamonds++;
        if (allDiamonds != null)
        {
            if (collectedDiamonds == allDiamonds.Length)
            {
                SetReward(+100f);
                EndEpisode();
            }
            else
                SetReward(+10f);
        }

        Debug.Log(GetCumulativeReward());
    }

    public void DamageWasTaken()
    {
        SetReward(-5f);
        Debug.Log(GetCumulativeReward());
    }

    public void PlayerHasLost()
    {
        SetReward(-100f);
        EndEpisode() ;
        Debug.Log(GetCumulativeReward());
    }

    public void PlayerWasDetected()
    {
        SetReward(-2f);
        Debug.Log(GetCumulativeReward());
    }

    private void CheckYaxis()
    {
        //in case player is launched above the playing area
        if(transform.localPosition.y > 20)
        {
            SetReward(-200f);
            EndEpisode();
        }
    }


    private void Update()
    {
        CheckYaxis();
        usedTime = Time.deltaTime;
    }
}
