using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Threading;
using System.Linq;

public class MLPlayerAgent : Agent
{
    [SerializeField] private Transform diamondTransform;

    [Header("Arrays")]
    //I need to invoke things to all NPCS
    public NPCMovement[] NPCmovement;
    public SpawningLocation[] spawningLocations;
    public PlayerSpawnPoint[] playerSpawningPoints;
    public Barrier[] barriers;
    Diamond[] allDiamonds;
    public Tile[] tiles;
    public List<bool> visitedTiles;
    Game game;


    [Header("Layer")]
    public int whatIsBarrierLayer;

    [Header("Other")]
    [SerializeField] public CharacterController controller;
    public AnimationStateController animationStateController;
    bool barrierPointReached;
    public float minDistanceToDiamond;
    Tile currentlyTouchedTile;

    public Vector3 startingPlayerPosition;


    int collectedDiamonds;
    float moveSpeed;
    float usedTime;

    public Vector3 moveDir;
    public float ySpeed;

    [Header("Grid related")]
    public Material gridMaterial;
    public Tile nextTileToGoTo;
    public Tile initialNextTileToGoTo;
    public bool checkInitialTile;
    public float distanceToNextTile;
    public bool properTileWasHit;
    public List<Tile> djikstraPath;
    public int currentPathTile;

    [Header("Steps")]
    public int maxStepsAfterReward;
    private int stepsAfterReward = 0;
    public int maxStepsBeforePenalty;
    List<float> allDistancesToDiamond;

    [Header("Velocity check")]
    public int maxStepsForVelocityMeasurement;
    private int currentStepsForVelocityMeasurement;
    public Vector3 initiPositionVel;
    public float smallVelocity;

    [Header("Rewards")]
    public float reachingDiamondReward;
    public float winningGameReward;
    public float distanceMultiplierReward;
    public float distanceReward;
    public float reachingBarrierPointReward;
    public float losingGameReward;
    public float hittingObstacleReward;
    public float hittingObstacleNoTriggerReward;
    public float reachingMaxStepReward;
    public float reachingMaxStepBeforePenaltyReward;
    public float timePenaltyMultiplierReward;
    public float newTileFoundReward;
    public float djikstraPathFindingReward;
    public float gettingDamageReward;
    public float smallVelocityReward;
    public float NPCseePlayerReward;
    public float playerIschasenAfterReward;



    int testNr;
    bool isTrainingOn;
    int randomIndexSpawn;
    [Header("For Additional Training")]
    public Transform diamondsTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private MeshRenderer floorMeshRender;

    public int GetRandomIndexSPawn()
    {
        return randomIndexSpawn;
    }

    private void UpdateRewards()
    {
        if(reachingDiamondReward == -1) reachingDiamondReward = 500;
        if (winningGameReward == -1) winningGameReward = 10000f;
        if (distanceMultiplierReward == -1) distanceMultiplierReward = 3f;
        if (distanceReward == -1) distanceReward = 15f;
        if (reachingBarrierPointReward == -1) reachingBarrierPointReward = 300f;
        if (losingGameReward == -1) losingGameReward = -3000f;
        if (hittingObstacleReward == -1) hittingObstacleReward = -300;
        if (hittingObstacleNoTriggerReward == -1) hittingObstacleNoTriggerReward = -1f;
        if (reachingMaxStepReward == -1) reachingMaxStepReward = -850f;
        if (timePenaltyMultiplierReward == -1) timePenaltyMultiplierReward = -1.5f;
        if (newTileFoundReward == -1) newTileFoundReward = 1f;
        if (djikstraPathFindingReward == -1) djikstraPathFindingReward = 40f;
        if (reachingMaxStepBeforePenaltyReward == 1) reachingMaxStepBeforePenaltyReward = -10f;
        if (NPCseePlayerReward == 1) NPCseePlayerReward = -0.01f;
    }

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

        //initially/ it was 2000
        if (maxStepsAfterReward == 0)
            maxStepsAfterReward = 2000;
        if (maxStepsBeforePenalty == 0)
            maxStepsBeforePenalty = 1000;

        game = GetComponentInParent<Game>();
        isTrainingOn = game.GetIsTrainingOn();
        testNr = game.GetTestNr();

        visitedTiles = new List<bool>();

        UpdateRewards();
        nextTileToGoTo = new Tile();
        properTileWasHit = false;
        checkInitialTile = true;
        currentPathTile = 0;

        currentStepsForVelocityMeasurement = 0;
        if (maxStepsForVelocityMeasurement == -1)
            maxStepsForVelocityMeasurement = 5;

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
            playerSpawningPoints = GetComponentInParent<Game>().GetPlayerSpawningPoints();
            int choice = Random.Range(0, playerSpawningPoints.Length);
            transform.position = playerSpawningPoints[choice].transform.position;
        }
        else
        {
            if (testNr == 2)
            {
                barriers = GetComponentInParent<Game>().GetBarriers();
                //only for TestEnv-1 !!
                if (barriers.Length > 0)
                    transform.localPosition = new Vector3(Random.Range(barriers[0].transform.localPosition.x - 4.45f, barriers[0].transform.localPosition.x + 3.83f),
                        1.5f, Random.Range(barriers[0].transform.localPosition.z + 1.51f, barriers[0].transform.localPosition.z + 4.96f));
                else
                    //transform.localPosition = new Vector3(Random.Range(-0.478f, 0.426f), 0f, Random.Range(2.51f, 6.37f));   //with respect to the barrier
                    transform.localPosition = new Vector3(Random.Range(-5.78f, 5.54f), 1.5f, Random.Range(-5.19f, 6.06f));       //whole plane rotation barrier
                                                                                                                                 //transform.localPosition = new Vector3(Random.Range(-7.14f, 8.57f), 1.5f, Random.Range(2.47f, 8.33f));
                                                                                                                                 //transform.localPosition = new Vector3(Random.Range(-3.79f, 4.06f), 1.5f, Random.Range(-3.3f, 4.31f));
            }
            else if (testNr == 3)
            {
                transform.localPosition = new Vector3(Random.Range(-8.52f, 7.76f), 1f, Random.Range(-10.88f, 7.09f));

            }
            else if (testNr == 4) {
                playerSpawningPoints = GetComponentInParent<Game>().GetPlayerSpawningPoints();
                randomIndexSpawn = Random.Range(0, playerSpawningPoints.Length);

                //initially I had "randomIndexSpawn'
                //randomIndexSpawn = 0;
                randomIndexSpawn = Random.Range(0, 2);
                if (randomIndexSpawn == 1) randomIndexSpawn = 4;
                //else if (randomIndexSpawn == 2) randomIndexSpawn = 5;

                transform.position =playerSpawningPoints[randomIndexSpawn].transform.position;

                //for curiosity driven rl
                visitedTiles = new List<bool>();

                tiles = GetComponentInParent<Game>().GetTiles();
                if (tiles != null)
                    for (int tile = 0; tile < tiles.Length; tile++)
                    {
                        visitedTiles.Add(false);
                        tiles[tile].ResetType();
                        if(tiles[tile].GetComponentInChildren<MeshRenderer>())
                            tiles[tile].GetComponentInChildren<MeshRenderer>().material = gridMaterial;
                    }

                //for the djikstra pathfinding
                //we need this position to fix issues related to the update of the next tile to go to
                //apparently, collision doesn't work properly when the episode starts
                if (GetComponentInParent<Game>().GetEnableDjikstraPathFinding())
                {

                    initialNextTileToGoTo = GetComponentInParent<Game>().GetNextTileToGoTo();
                    startingPlayerPosition = transform.position;
                    currentPathTile = 0;
                    checkInitialTile = true;

                    UpdateNextTileToGoTo(true);
                }

                allDistancesToDiamond = new List<float>();

            }

            allDiamonds = GetComponentInParent<Game>().GetDiamonds();
            minDistanceToDiamond = Vector3.Distance(allDiamonds[0].transform.position, transform.position);
        }

        controller.enabled = true;

        //changing the rotation of the barriers
        if (isTrainingOn)
        {
            barriers = GetComponentInParent<Game>().GetBarriers();
            for (int barrier = 0; barrier < barriers.Length; barrier++)
            {
                if (testNr == 2)
                    barriers[barrier].transform.rotation = Quaternion.Euler(0, Random.Range(0, 90), 0);
                else if (testNr == 3)
                {
                    int[] angles = { 0, 45, 90 };
                    Vector3[] scales = { new Vector3(8, 4, 1), new Vector3(7, 3, 1), new Vector3(6, 3, 1), new Vector3(5, 3, 1), new Vector3(6, 4.5f, 1) };

                    int randomIndex = Random.Range(0, angles.Length);
                    barriers[barrier].transform.rotation = Quaternion.Euler(0, angles[randomIndex], 0);

                    int randomIndexScale = Random.Range(0, scales.Length);
                    barriers[barrier].transform.localScale = scales[randomIndexScale];

                        //barriers[barrier].transform.localPosition = new Vector3(Random.Range(-7, 7), 1, Random.Range(-10, 7));
                    

                }
                else if (testNr == 4)
                {
                    


                }
            }
        }

        //resetting properties
        GetComponent<ThirdPersonMovement>().ResetCurrentHealth();
        GetComponent<PlayerInventory>().ResetProperties();

        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCmovement[npc].ResetProperties();

        if (isTrainingOn)
        {
            //reset the positions of diamonds
            for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
                allDiamonds[diamond].ResetPosition();
        }

        //for training
        barrierPointReached = false;

    }

    void UpdateNextTileToGoTo(bool additionalCondition)
    {
        //update the color
        if(djikstraPath.Count > 0)
        {
            foreach(Tile tile in djikstraPath)
                if (tile.GetComponentInChildren<MeshRenderer>())
                    tile.GetComponentInChildren<MeshRenderer>().material = game.pathMaterial;
        }

        List<Tile> djikstraPathVar = GetComponentInParent<Game>().CreateDjikstraPath();

        if (djikstraPathVar.Count > 0)
        {

            if (initialNextTileToGoTo != null)
            {
                //if the distance bewteen the initial position of the player and current next tile is greater than 5, immediately change
                //the next tile
                if (checkInitialTile && Vector3.Distance(startingPlayerPosition, initialNextTileToGoTo.transform.position) > 5)
                {
                    djikstraPath = new List<Tile>();
                    djikstraPath = djikstraPathVar;

                    if (djikstraPath.Count > 0)
                        if (Vector3.Distance(transform.position, djikstraPath[0].transform.position) < 3)
                        {
                            checkInitialTile = false;
                            currentPathTile = 0;
                            nextTileToGoTo = djikstraPath[currentPathTile];
                        }

                    return;
                }
            }
            else
            {
                djikstraPath = new List<Tile>();
                djikstraPath = djikstraPathVar;
                if (djikstraPath.Count > 0)
                    initialNextTileToGoTo = djikstraPath[0];
            }


        }

        if (additionalCondition)
        {
            djikstraPath = new List<Tile>();
            djikstraPath = djikstraPathVar;
        }

        if(djikstraPath.Count > 0)
            nextTileToGoTo = djikstraPath[currentPathTile];


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

        //sensor.AddObservation(GetGamesTransformPosition(transform.position));

        //Debug.Log(GetGamesTransformPosition(transform.position) + " and " + transform.localPosition);
        //adding the diamond position with the closest distance
        int minDiamondIndex = 0;
        Vector3 difference = (GetGamesTransformPosition(allDiamonds[minDiamondIndex].transform.position) - GetGamesTransformPosition(transform.position)).normalized;
        float minDistance = Vector3.Distance(GetGamesTransformPosition(allDiamonds[minDiamondIndex].transform.position), GetGamesTransformPosition(transform.position));

        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
        {

            //instead of position, add x and y distances
            float distanceCheck = Vector3.Distance(GetGamesTransformPosition(allDiamonds[diamond].transform.position), GetGamesTransformPosition(transform.position));
            if (distanceCheck < minDistance) { 
                minDistance = distanceCheck;
                minDiamondIndex = diamond;
            
            }


           
        }
        //instead of position, add x and y distances
        difference = (GetGamesTransformPosition(allDiamonds[minDiamondIndex].transform.position) - GetGamesTransformPosition(transform.position)).normalized;

        sensor.AddObservation(difference.x);
        sensor.AddObservation(difference.z);

        //adding position of NPCS
        //for (int npc = 0; npc < NPCmovement.Length; npc++)
        //sensor.AddObservation(GetGamesTransformPosition(NPCmovement[npc].transform.position));

        //add the tiles from Djikstra path finding
        if (GetComponentInParent<Game>().GetEnableDjikstraPathFinding())
        {
            //we need to make sure that all Tiles has executed OnTriggeEnter before we proceed
            //or just create a new Djikstra Method that will start with current position, update all Tiles and then proceed:
            // - find the tile where the player is
            //- find the tiles where diamonds are
            //- get the path
            //- get the next tile in the path

            UpdateNextTileToGoTo(false);

                float distanceToCurrentTile = Vector3.Distance(nextTileToGoTo.transform.position, transform.position);
                sensor.AddObservation(GetGamesTransformPosition(nextTileToGoTo.transform.position));
                sensor.AddObservation(distanceToCurrentTile);
                distanceToNextTile = distanceToCurrentTile;     //for debugging
            
            //else
            //{
            //    float distance = Vector3.Distance(allDiamonds[0].transform.position, transform.position);
            //    sensor.AddObservation(GetGamesTransformPosition(allDiamonds[0].transform.position));
            //    sensor.AddObservation(distance);
            //}
        }

    }

    void ResetDiamonds()
    {
        GetComponentInParent<Game>().ResetDiamonds();

        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            allDiamonds[diamond].ResetProperties();
    }

    void CheckVelocity()
    {
        currentStepsForVelocityMeasurement++;

        if (currentStepsForVelocityMeasurement == 1)
        {
            initiPositionVel = GetGamesTransformPosition( transform.position);
        }
        else if (currentStepsForVelocityMeasurement == maxStepsForVelocityMeasurement) { 
        
            Vector3 currentPosition = GetGamesTransformPosition(transform.position);
            float takenDistance = Vector3.Distance(currentPosition, initiPositionVel);

            float velocity = takenDistance / maxStepsForVelocityMeasurement * 100;

            //Debug.Log("Current velocity " + velocity);

            if (smallVelocity != 0)
                if (velocity < smallVelocity)
                    SetReward(+smallVelocityReward);

            currentStepsForVelocityMeasurement = 0;
        }
            
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
        if (isTrainingOn)
        {
            allDiamonds = GetComponentInParent<Game>().GetDiamonds();
            //reward for making the distance to diamond smaller
            float newDistanceToDiamond = Vector3.Distance(allDiamonds[0].transform.position, transform.position);
            if(allDistancesToDiamond.Count == 0)
                allDistancesToDiamond.Add(newDistanceToDiamond);

            //Debug.Log("test " + 1 / newDistanceToDiamond * 100);        //20 - 35 
            if (newDistanceToDiamond < allDistancesToDiamond.Min())
            {
                allDistancesToDiamond.Add(newDistanceToDiamond);

                //SetReward(+ 1/newDistanceToDiamond * distanceMultiplierReward);
                SetReward(+distanceReward);
                minDistanceToDiamond = newDistanceToDiamond;
            }
            else
            {
                //give a time penalty
                //SetReward(+ 1 / newDistanceToDiamond * distanceMultiplierReward * timePenaltyMultiplierReward);
            }
        }

        //if the reward wasn't collected during the episode, reset it
        stepsAfterReward++;
        if (stepsAfterReward == maxStepsAfterReward)
        {
            SetReward(+reachingMaxStepReward);
            stepsAfterReward = 0;
            ResetDiamonds();
            if (isTrainingOn && floorMeshRender != null)
                floorMeshRender.material = neutralMaterial;
            EndEpisode();
        }
        else if(stepsAfterReward >= maxStepsBeforePenalty)
        {
            SetReward(+reachingMaxStepBeforePenaltyReward);
        }

        //check velocity
        CheckVelocity();
        

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

        SetReward(+reachingDiamondReward);
        if (isTrainingOn && floorMeshRender != null)
        {
            floorMeshRender.material = winMaterial;
        }
        EndEpisode();


    }

    public void PlayerHasWon()
    {
        SetReward(+winningGameReward);
        Debug.Log(GetCumulativeReward());
        if (isTrainingOn && floorMeshRender != null)
        {
            floorMeshRender.material = winMaterial;
        }
        EndEpisode();
        
    }

    public void DamageWasTaken()
    {
        SetReward(+gettingDamageReward);
        Debug.Log(GetCumulativeReward());
    }

    public void PlayerHasLost()
    {
        SetReward(+losingGameReward);
        Debug.Log(GetCumulativeReward());
        ResetDiamonds();

        if (isTrainingOn && floorMeshRender != null)
            floorMeshRender.material = loseMaterial;

        EndEpisode() ;
        
    }

    public void PlayerDetectionCheck(bool canNPCSeePlayer)
    {
        if (canNPCSeePlayer)
        {
            SetReward(+NPCseePlayerReward);
            Debug.Log("player was spotted");
        }
    }

    public void PlayerIsChasenAfter()
    {
        SetReward(+playerIschasenAfterReward);
        EndEpisode();
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
    private void OnCollisionEnter(Collision collision)
    {
        ////colliding with "what is barrier"
        if (collision.gameObject.layer == whatIsBarrierLayer)
        {

            //obstacle was hit
            SetReward(+hittingObstacleNoTriggerReward);
            ResetDiamonds();
            if (isTrainingOn && floorMeshRender != null)
            {
                floorMeshRender.material = loseMaterial;
            }

            Debug.Log("barrier hit");
            EndEpisode();

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ////colliding with "what is barrier"
        if (other.gameObject.layer == whatIsBarrierLayer)
        {

            //obstacle was hit
            SetReward(+hittingObstacleReward);
            ResetDiamonds();
            if (isTrainingOn && floorMeshRender != null)
            {
                floorMeshRender.material = loseMaterial;
            }
            Debug.Log("trigger obstacle hit");
            EndEpisode();

        }
        //when tile is hit
        //for curiosity driven rl
        else if (other.gameObject.GetComponent<Tile>())
        {
            //for curiosity driven rl
            tiles = GetComponentInParent<Game>().GetTiles();
            int tile = 0;
            //find which tile was hit
            for (tile = 0; tile < tiles.Length; tile++)
                if (tiles[tile].transform.position == other.transform.position)
                    break;

            //check is the tile was already visited
            if (tile < tiles.Length)
            {
                if (visitedTiles[tile] == false)
                {
                    visitedTiles[tile] = true;
                    SetReward(+newTileFoundReward);

                    //Debug.Log("Tile reward " + tile);
                }
            }

            //for Enabled Djikstra path finding
            //when next proper Tile was hit
            if (nextTileToGoTo != null)
            {
                if (GetComponentInParent<Game>().GetEnableDjikstraPathFinding() && other.gameObject.transform.position == nextTileToGoTo.transform.position)
                {
                    Debug.Log("Proper Tile hit");
                    SetReward(+djikstraPathFindingReward);
                    properTileWasHit = true;

                    //if the tile that was hit isn't last, increase the number
                    if (djikstraPath[currentPathTile] != djikstraPath.Last())
                        currentPathTile++;

                }
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
