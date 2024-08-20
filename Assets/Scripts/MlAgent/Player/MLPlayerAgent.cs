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
    bool barrierPointReached;
    float distanceToDiamond;

    public Vector3 startingPlayerPosition;

    
    int collectedDiamonds;
    float moveSpeed;
    float usedTime;

    public Vector3 moveDir;
    public float ySpeed;

    [Header("Steps")]
    private int stepsAfterReward = 0;
    public int maxStepsAfterReward;

    [Header("Rewards")]
    float reachingDiamondReward = 700f;
    float winningGameReward = 5000f;
    float distanceMultiplierReward = 10f;
    float reachingBarrierPointReward = 300f;
    float losingGameReward = -3000f;
    float hittingObstacleReward = -1200;
    float reachingMaxStepReward = -270f;
    float timePenaltyMultiplierReward = - 1.2f;


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
            barriers = GetComponentInParent<Game>().GetBarriers();
            //only for TestEnv-1 !!
            transform.localPosition = new Vector3(Random.Range(barriers[0].transform.localPosition.x - 4.45f, barriers[0].transform.localPosition.x + 3.83f), 
                1.5f, Random.Range(barriers[0].transform.localPosition.z + 1.51f, barriers[0].transform.localPosition.z + 4.96f));

            //transform.localPosition = new Vector3(Random.Range(-0.478f, 0.426f), 0f, Random.Range(2.51f, 6.37f));   //with respect to the barrier
            //transform.localPosition = new Vector3(Random.Range(-5.78f, 5.54f), 1.5f, Random.Range(-5.19f, 6.06f));       //whole plane rotation barrier
            //transform.localPosition = new Vector3(Random.Range(-7.14f, 8.57f), 1.5f, Random.Range(2.47f, 8.33f));
            //transform.localPosition = new Vector3(Random.Range(-3.79f, 4.06f), 1.5f, Random.Range(-3.3f, 4.31f));

            allDiamonds = GetComponentInParent<Game>().GetDiamonds();
            distanceToDiamond = Vector3.Distance(allDiamonds[0].transform.position, transform.position);
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

        if (isTrainingOn)
        {
            //reset the positions of diamonds
            for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
                allDiamonds[diamond].ResetPosition();
        }

        //for training
        barrierPointReached = false;

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

        //getting closer to diamond
        //disable it once agent knows how to reach the diamond
        if (isTrainingOn && barrierPointReached)
        {
            //reward for making the distance to diamond smaller
            float newDistanceToDiamond = Vector3.Distance(allDiamonds[0].transform.position, transform.position);

            //Debug.Log("test " + 1 / newDistanceToDiamond * 100);        //20 - 35 
            if (newDistanceToDiamond < distanceToDiamond)
            {
                SetReward(+ 1/newDistanceToDiamond * distanceMultiplierReward);
                distanceToDiamond = newDistanceToDiamond;
            }
            else
            {
                //give a time penalty
                SetReward(+ 1 / newDistanceToDiamond * distanceMultiplierReward * timePenaltyMultiplierReward);
            }
        }

        //if the reward wasn't collected during the time of the step, reset the episode
        stepsAfterReward++;
        if (stepsAfterReward == maxStepsAfterReward)
        {
            SetReward(reachingMaxStepReward);
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

        SetReward(reachingDiamondReward);
        if (isTrainingOn)
        {
            floorMeshRender.material = winMaterial;
        }
        EndEpisode();


    }

    public void PlayerHasWon()
    {
        SetReward(winningGameReward);
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
        SetReward(losingGameReward);
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
            //if the player collides with the point that is next to the barrier, give him the reward
            if (other.gameObject.GetComponent<BarrierPoint>())
            {
                if (!barrierPointReached)
                {
                    SetReward(reachingBarrierPointReward);
                    Debug.Log("Barrier Point collision");
                    barrierPointReached = true;
                }
            }
            else
            {
                //obstacle was hit
                SetReward(hittingObstacleReward);
                ResetDiamonds();
                if (isTrainingOn)
                {
                    floorMeshRender.material = loseMaterial;
                }
                EndEpisode();
            }
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
