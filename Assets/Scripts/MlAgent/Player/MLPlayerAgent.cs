using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MLPlayerAgent : Agent
{
    [SerializeField] private Transform diamondTransform;

    [Header("Arrays")]
    //I need to invoke things to all NPCS
    public NPCMovement[] NPCmovement;
    public SpawningLocation[] spawningLocations;

    [Header("Layer")]
    public int whatIsBarrierLayer;

    [Header("Other")]
    [SerializeField] public CharacterController controller;
    public AnimationStateController animationStateController;

    public Vector3 startingPlayerPosition;
    public List<Vector3> diamondsPositions;
    public List<Vector3> NPCPositions;

    Diamond[] allDiamonds;
    int collectedDiamonds;
    float moveSpeed;
    float usedTime;

    public Vector3 moveDir;
    public float ySpeed;

    [Header("Steps")]
    private int stepsAfterReward = 0;
    public int maxStepsAfterReward = 4000;

    [Header("For Additional Training")]
    public bool isTrainingOn;
    public Transform diamondsTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRender;


    void Start()
    {
        //needs to be dynamic
        diamondsPositions = new List<Vector3>();
        NPCPositions = new List<Vector3>();

        //for the player
        //new Vector3 (-0.11f, -2f, -42.56f)

        startingPlayerPosition = transform.localPosition;

        //Debug.Log("Starting position: " + startingPlayerPosition);

        collectedDiamonds = 0;
        controller = GetComponent<CharacterController>();
        usedTime = Time.deltaTime;

        animationStateController = GetComponentInChildren<AnimationStateController>();

        allDiamonds = GetComponentInParent<Game>().GetDiamonds();

        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        //important! I have an absolute posiotion here (with respect to the environment), not a local one (with respect to the Diamonds object) as in the inspecotr
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            diamondsPositions.Add(allDiamonds[diamond].transform.position);

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCPositions.Add(NPCmovement[npc].transform.position);

        //layer related
        whatIsBarrierLayer = 9;


    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("New Episode");
        stepsAfterReward = 0;
        //we need to disbale controller to avoid collisions
        controller.enabled = false;

        //randomize spawning point
        if (!isTrainingOn)
        {
            spawningLocations = GetComponentInParent<Game>().GetSpawningLocations();
            int choice = Random.Range(0, spawningLocations.Length);
            transform.position = spawningLocations[choice].transform.position;
        }
        else
        {
            transform.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 1.5f, Random.Range(-3.0f, 3.0f));
        }
        Debug.Log("New position: " + transform.localPosition);

        controller.enabled = true;


        //resetting properties
        GetComponent<ThirdPersonMovement>().ResetCurrentHealth();
        GetComponent<PlayerInventory>().ResetProperties();

        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        //important! I have an absolute posiotion here (with respect to the environment), not a local one (with respect to the Diamonds object) as in the inspecotr
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            diamondsPositions.Add(allDiamonds[diamond].transform.localPosition);

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCPositions.Add(NPCmovement[npc].transform.localPosition);

        
        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCmovement[npc].ResetPropertiesCall();

        
        

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //adding all of the diamond positions
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            sensor.AddObservation(diamondsPositions[diamond]);

        //adding position of NPCS
        for (int npc = 0; npc < NPCmovement.Length; npc++)
            sensor.AddObservation(NPCPositions[npc]);

    }

    void ResetDiamonds()
    {
        GetComponentInParent<Game>().ResetDiamonds();

        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            allDiamonds[diamond].ResetProperties();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool jump = actions.ContinuousActions[2] >=-1.0f && actions.ContinuousActions[2] <-0.5f? true : false;
        bool sneak = actions.ContinuousActions[2] >= -0.5f && actions.ContinuousActions[2] < 0.0f ? true : false;
        bool sprint = actions.ContinuousActions[2] >= 0.0f && actions.ContinuousActions[2] < 0.5f ? true : false;

        
        //Debug.Log("Discrete action: " + actions.ContinuousActions[2]);

        //simply apply movement from ThirdPersonMovement
        GetComponent<ThirdPersonMovement>().ApplyMovement(moveX, moveZ, jump, sprint, sneak, false);

        //if the reward wasn't collected during the time of the step, reset the episode
        stepsAfterReward++;
        if (stepsAfterReward == maxStepsAfterReward)
        {
            SetReward(-20f);
            stepsAfterReward = 0;
            ResetDiamonds();
            EndEpisode();
        }
        

    }



    //Heuristics are mental shortcuts for solving problems in a quick way that delivers a result that is sufficient enough to be useful given time constraints
    //Implement this function to provide custom decision making logic or to support manual control of an agent using keyboard, mouse, game controller input, or a script.
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //Debug.Log("Length: " + continuousActions.Length);
        continuousActions[0] = Input.GetAxisRaw("Horizontal"); //x
        continuousActions[1] = Input.GetAxisRaw("Vertical"); //z

        //I will encode discrete actions as continuous ones
        float discreteActionValue = 0.0f;

        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionValue = -1.0f; // Jump
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            discreteActionValue = -0.5f; // Crouch
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            discreteActionValue = 0.5f; // Sprint
        }
        else
        {
            discreteActionValue = 1.0f; // No action (or default)
        }

        continuousActions[2] = discreteActionValue;


    }

    public void DiamondWasCollected()
    {
        collectedDiamonds++;
        stepsAfterReward = 0;

        SetReward(+100f);
        Debug.Log(GetCumulativeReward());

        
    }

    public void PlayerHasWon()
    {
        SetReward(+1000f);
        Debug.Log(GetCumulativeReward());
        if (isTrainingOn)
        {
            floorMeshRender.material = winMaterial;
        }
        EndEpisode();
        
    }

    public void DamageWasTaken()
    {
        SetReward(-10f);
        Debug.Log(GetCumulativeReward());
    }

    public void PlayerHasLost()
    {
        SetReward(-300f);
        Debug.Log(GetCumulativeReward());
        ResetDiamonds();

        if(isTrainingOn)
            floorMeshRender.material = loseMaterial;
        EndEpisode() ;
        
    }

    public void PlayerWasDetected()
    {
        SetReward(-30f);
        //Debug.Log(GetCumulativeReward());
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

    private void OnTriggerEnter(Collider other)
    {
        //colliding with "what is barrier"
        if (other.gameObject.layer == whatIsBarrierLayer)
        {
            SetReward(-15f);
            ResetDiamonds();
            EndEpisode();
        }

    }
    private void Update()
    {
        CheckYaxis();
        usedTime = Time.deltaTime;

        if(transform.localPosition.y < -15)
        {
            PlayerHasLost();
        }
        //Debug.Log(GetCumulativeReward());
    }
}
