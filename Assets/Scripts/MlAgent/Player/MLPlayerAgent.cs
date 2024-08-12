using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Threading;

public class MLPlayerAgent : Agent
{
    [SerializeField] private Transform diamondTransform;

    [Header("Arrays")]
    //I need to invoke things to all NPCS
    public NPCMovement[] NPCmovement;
    public SpawningLocation[] spawningLocations;
    public Barrier[] barriers;
    Diamond[] allDiamonds;
    Game game;


    [Header("Layer")]
    public int whatIsBarrierLayer;

    [Header("Other")]
    [SerializeField] public CharacterController controller;
    public AnimationStateController animationStateController;

    public Vector3 startingPlayerPosition;

    
    int collectedDiamonds;
    float moveSpeed;
    float usedTime;

    public Vector3 moveDir;
    public float ySpeed;

    [Header("Steps")]
    private int stepsAfterReward = 0;
    public int maxStepsAfterReward;

    [Header("For Additional Training")]
    public bool isTrainingOn;
    public Transform diamondsTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private MeshRenderer floorMeshRender;


    void Start()
    {

        //for the player
        //new Vector3 (-0.11f, -2f, -42.56f)

        startingPlayerPosition = transform.localPosition;

        //Debug.Log("Starting position: " + startingPlayerPosition);

        collectedDiamonds = 0;
        controller = GetComponent<CharacterController>();
        usedTime = Time.deltaTime;

        animationStateController = GetComponentInChildren<AnimationStateController>();

        //layer related
        whatIsBarrierLayer = 9;

        maxStepsAfterReward = 1500;
        

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
            transform.localPosition = new Vector3(Random.Range(-5.78f, 5.54f), 1.5f, Random.Range(-5.19f, 6.06f));       //whole plane rotation barrier
            //transform.localPosition = new Vector3(Random.Range(-7.14f, 8.57f), 1.5f, Random.Range(2.47f, 8.33f));
            //transform.localPosition = new Vector3(Random.Range(-3.79f, 4.06f), 1.5f, Random.Range(-3.3f, 4.31f));
        }

        controller.enabled = true;

        //changing the rotation of the barrier
        if (isTrainingOn)
        {
            barriers = GetComponentInParent<Game>().GetBarriers();
            for (int barrier = 0; barrier < barriers.Length; barrier++)
                barriers[barrier].transform.rotation = Quaternion.Euler(0, Random.Range(0, 90), 0);
        }

        //resetting properties
        GetComponent<ThirdPersonMovement>().ResetCurrentHealth();
        GetComponent<PlayerInventory>().ResetProperties();

        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCmovement[npc].ResetPropertiesCall();

        
        

    }

    public Vector3 GetGamesTransformPosition(Vector3 position)
    {
        game = GetComponentInParent<Game>();
        return game.transform.InverseTransformPoint(position);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        sensor.AddObservation(GetGamesTransformPosition(transform.position));

        //Debug.Log(GetGamesTransformPosition(transform.position) + " and " + transform.localPosition);
        //adding all of the diamond positions
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
        {
            sensor.AddObservation(GetGamesTransformPosition(allDiamonds[diamond].transform.position));
            //Debug.Log("From observatiopns: " + GetGamesTransformPosition(allDiamonds[diamond].transform.position));
        }

        //adding position of NPCS
        for (int npc = 0; npc < NPCmovement.Length; npc++)
            sensor.AddObservation(GetGamesTransformPosition(NPCmovement[npc].transform.position));


        //adding barrier detection
        //I will use raycast for this
        //float rayLength = 10.0f;
        //int numRays = 8;
        //for (int i = 0; i < numRays; i++)
        //{
        //    float angle = (i / (float)numRays) * 360f;
        //    Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, direction, out hit, rayLength))
        //    {
        //        sensor.AddObservation(hit.distance); // Add distance to hit object
        //    }
        //    else
        //    {
        //        sensor.AddObservation(rayLength); // Max distance if nothing is hit
        //    }

        //}

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
        GetComponent<ThirdPersonMovement>().ApplyMovement(moveX, moveZ, jump, sprint, sneak, false, 1f);

        //if the reward wasn't collected during the time of the step, reset the episode
        stepsAfterReward++;
        if (stepsAfterReward == maxStepsAfterReward)
        {
            SetReward(-20f);
            stepsAfterReward = 0;
            ResetDiamonds();
            if (isTrainingOn)
                floorMeshRender.material = neutralMaterial;
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
        if (isTrainingOn)
        {
            floorMeshRender.material = winMaterial;
        }
        EndEpisode();


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
            SetReward(-200f);
            ResetDiamonds();
            if (isTrainingOn)
            {
                floorMeshRender.material = loseMaterial;
            }
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
