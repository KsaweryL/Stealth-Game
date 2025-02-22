using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Threading;
using System.Linq;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class MLPlayerAgent : Agent
{
    [SerializeField] private Transform diamondTransform;

    [Header("Arrays")]
    //I need to invoke things to all NPCS
    public NPCMovement[] NPCmovement;
    public SpawningLocation[] spawningLocations;
    public PlayerSpawnPoint[] playerSpawningPoints;
    public Barrier[] barriers;
    public Diamond[] allDiamonds;
    public Diamond[] allCurrentDiamonds;
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
    public bool isPauseOn;

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
    public int currentPathTile;

    [Header("Steps")]
    public int maxStepsAfterReward;
    private int stepsAfterReward = 0;
    public int maxStepsBeforePenalty;
    List<float> allDistancesToDiamond;
    List<float> allDistancesToNPC;
    List<float> allDistancesToHidingSpot;

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
    public float eachStepTakenReward;
    public float timePenaltyMultiplierReward;
    public float newTileFoundReward;
    public float gettingDamageReward;
    public float smallVelocityReward;
    public float NPCseePlayerReward;
    public float playerIsHiddenReward;
    public float playerIsHiddenAndWithinNPCFOVReward;
    public float playerIschasedAfterReward;
    public float distanceToClosestNPCReward;
    public float distanceToClosestHidingSpotReward;
    public float nextNavMeshWaypointReached;
    public float waypointDistanceReward;
    public float waypointInreasingDistanceReward;

    int testNr;
    bool isTrainingOn;
    int randomIndexSpawn;
    [Header("For Additional Training")]
    public Transform diamondsTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private MeshRenderer floorMeshRender;
    [SerializeField] private Image imageRender;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color neutralColor = Color.white;
    [SerializeField] private Color loseColor = Color.red;
    public bool spectating;
    public bool overfit;

    [Header("NPC")]
    public NPCMovement chosenNPC;
    public float minDistanceToNPC;

    [Header("Hiding spot")]
    public HidingSpotArea[] hidingSpotAreas;
    public HidingSpot[] hidingSpots;
    public HidingSpotArea chosenHidingSpotArea;
    public HidingSpot chosenHidingSpot;
    public float minDistanceToHidingSpot;
    

    [Header("Diamond")]
    public Diamond chosenDiamond;

    [Header("NavMesh")]
    public NavMeshAgent navMeshAgent;
    public int currentWaypointIndex = 0;
    public List<Vector3> pathCorners;
    public float navMeshWaypointDistance;
    Vector3 nextWaypoint;
    List<float> allDistancesToWaypoint;
    public float minDistanceToWaypoint;
    public GameObject objectToSpawn;        //object to debug
    //public GameObject objectToSpawn2;
    //public GameObject objectToSpawn3;

    [Header("Line rendering")]
    public bool enableLineRendering;
    private LineRenderer lineRenderer;
    List<GameObject> lineRenderers;
    public int currentLine;
    public int numberOfLineRendererPoints = 2;
    private float lineRendererTimer = 0f;
    private Vector3 previousLineRendererPosition;
    bool stopLineRendering;

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
        if (reachingMaxStepBeforePenaltyReward == 1) reachingMaxStepBeforePenaltyReward = -10f;
        if (NPCseePlayerReward == 1) NPCseePlayerReward = -0.01f;
    }

    
    void Start()
    {

        //for the player
        //new Vector3 (-0.11f, -2f, -42.56f)

        startingPlayerPosition = transform.position;

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

        //restart the time values
        if(isTrainingOn)
            if (GetComponent<Metrics>())
                GetComponent<Metrics>().UpdateInitialValues();


        //needs reset after changing scenes
        isPauseOn = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshUpdate();
        OnEpisodeBegin();
    }

    void NavMeshUpdate()
    {
        //update navmesh
        allCurrentDiamonds = GetComponentInParent<Game>().GetDiamonds();
        if (allCurrentDiamonds.Length > 0)
        {
            //choose the closest diamond
            Diamond currentlyChosenDiamond = allCurrentDiamonds[0];
            if (allCurrentDiamonds.Length > 0)
            {

                foreach (Diamond diamond in allCurrentDiamonds)
                {
                    if (Vector3.Distance(diamond.transform.position, transform.position) < Vector3.Distance(currentlyChosenDiamond.transform.position, transform.position))
                        currentlyChosenDiamond = diamond;
                }
            }

            //navMeshAgent.destination = currentlyChosenDiamond.transform.position;
            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(currentlyChosenDiamond.transform.position, path);

            //Debug.Log(currentlyChosenDiamond.gameObject.name + " and " + currentlyChosenDiamond.transform.localPosition);
            //pathCorners = path.corners;

            int divideNumberOfCorners = 1;
            pathCorners = new List<Vector3>();

            if (path.corners.Length > 1)
            {
                for (int corner = 1; corner < path.corners.Length; corner += divideNumberOfCorners)
                {
                    pathCorners.Add(path.corners[corner]);
                }

                //add the last one if it's not added
                if (path.corners[path.corners.Length-1] != pathCorners[pathCorners.Count-1])
                    pathCorners.Add(path.corners[path.corners.Length - 1]);
            }
            else
                pathCorners.Add(path.corners[0]);

            //for initial training
            //pathCorners = new List<Vector3>() { currentlyChosenDiamond.transform.position };

            //Debug.Log("path corners length " + pathCorners.Length);

            currentWaypointIndex = 0;
            if (pathCorners.Count > 1)
                currentWaypointIndex = 1;
            else { currentWaypointIndex = 0; }


            //debug
            //objectToSpawn.transform.position = pathCorners[pathCorners.Length - 1];
            //objectToSpawn3.transform.position = currentlyChosenDiamond.transform.position;

            navMeshAgent.updateRotation = false;

            allDistancesToWaypoint = new List<float>();

            //update the diamond
            if(chosenDiamond != currentlyChosenDiamond)
                allDistancesToDiamond = new List<float>();

            chosenDiamond = currentlyChosenDiamond;
        }
    }

    public override void OnEpisodeBegin()
    {
        //in case sth wasn't loaded
        if (!controller)
        {
            Start();
        }

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
                //randomIndexSpawn = 5;
                randomIndexSpawn = Random.Range(0, 2);
                if (randomIndexSpawn == 0) randomIndexSpawn = 5;
                if (randomIndexSpawn == 1) randomIndexSpawn = 2;
                //else if (randomIndexSpawn == 2) randomIndexSpawn = 5;


                if (spectating || overfit)
                    transform.position = startingPlayerPosition;
                else
                    transform.position = playerSpawningPoints[randomIndexSpawn].transform.position;

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

                allDistancesToDiamond = new List<float>();
                allDistancesToNPC = new List<float>();
                allDistancesToHidingSpot = new List<float>();

            }

        }

        controller.enabled = true;

        //resetting properties
        GetComponent<ThirdPersonMovement>().ResetCurrentHealth();
        GetComponent<PlayerInventory>().ResetProperties();

        //update navmesh
        navMeshAgent = GetComponent<NavMeshAgent>();
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = true;

        if (allDiamonds.Length == 0)
        {
            allDiamonds = GetComponentInParent<Game>().GetDiamonds();
            minDistanceToDiamond = Vector3.Distance(allDiamonds[0].transform.position, transform.position);


        }

        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();

        for (int npc = 0; npc < NPCmovement.Length; npc++)
            NPCmovement[npc].ResetProperties();

        if (isTrainingOn)
        {
            ResetDiamonds();
            if(FindObjectOfType<InventoryUI>())
                FindObjectOfType<InventoryUI>().ResetDiamondText();

            if (GetComponent<Metrics>())
                GetComponent<Metrics>().UpdateStartTime();
        }


            //for training
            barrierPointReached = false;

        //for line renderer
        if(GetComponent<LineRenderer>())
            SetInitialValuesLineRenderer();

        

    }

    public Vector3 GetGamesTransformPosition(Vector3 position)
    {
        game = GetComponentInParent<Game>();
        return game.transform.InverseTransformPoint(position);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        

        allCurrentDiamonds = GetComponentInParent<Game>().GetDiamonds();
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();
        hidingSpotAreas = GetComponentInParent<Game>().GetHidingSpotAreas();
        hidingSpots  =GetComponentInParent<Game>().GetHidingSpots();

        if (allCurrentDiamonds.Length > 0)
        {
            if (chosenDiamond == null)
            {
                chosenDiamond = allCurrentDiamonds[0];
            }

            ////instead of position, add x and y distances
            Vector3 difference = (GetGamesTransformPosition(chosenDiamond.transform.position) - GetGamesTransformPosition(transform.position)).normalized;

            //sensor.AddObservation(difference.x);
            //sensor.AddObservation(difference.z);

            //update navmesh
            if (chosenDiamond != null && pathCorners.Count > 1)
                nextWaypoint = pathCorners[currentWaypointIndex];
            else
                nextWaypoint = chosenDiamond.transform.position;

            Vector3 differenceToNextWaypoint = (GetGamesTransformPosition(nextWaypoint) - GetGamesTransformPosition(transform.position)).normalized;

            //debug
            //Debug.Log("Waypoint location " + GetGamesTransformPosition(nextWaypoint));

            objectToSpawn.transform.position = new Vector3(nextWaypoint.x, 1f, nextWaypoint.z);
            navMeshWaypointDistance = Vector3.Distance(transform.position, nextWaypoint);


            sensor.AddObservation(differenceToNextWaypoint.x);
            sensor.AddObservation(differenceToNextWaypoint.z);

            //sensor.AddObservation(GetGamesTransformPosition(transform.position));
            //sensor.AddObservation(GetGamesTransformPosition(nextWaypoint));


            //adding position of NPCS
            if (NPCmovement.Length > 10)
                throw new System.Exception("There are too many NPCS.");
            //else if (NPCmovement.Length == 0)
            //    Debug.Log("Therer are np NPC detected");

            //we will also add observation whether the player is being spotted or not
            bool playerIsSpotted = false;

            for (int npc = 0; npc < NPCmovement.Length; npc++)
            {
                Vector3 difference_npc = (GetGamesTransformPosition(GetGamesTransformPosition(NPCmovement[npc].transform.position)) - GetGamesTransformPosition(transform.position)).normalized;

                sensor.AddObservation(difference_npc.x);
                sensor.AddObservation(difference_npc.z);

                //add velocities of each guard
                sensor.AddObservation(NPCmovement[npc].GetComponent<NavMeshAgent>().velocity.x);
                sensor.AddObservation(NPCmovement[npc].GetComponent<NavMeshAgent>().velocity.z);

                //update playerIsSpotted variable
                //Debug.Log("npcfov " + NPCmovement[npc].GetComponent<NPCFieldOfView>().GetCanSeePlayerNPCFOV());
                if (NPCmovement[npc].GetComponent<NPCFieldOfView>().GetCanSeePlayerNPCFOV())
                {
                    playerIsSpotted = true;
                    Debug.Log("player was spotted");

                }
            }

            sensor.AddObservation(playerIsSpotted);

            //adding positions of the nearest hiding spot area
            if (hidingSpotAreas.Length > 15)
                throw new System.Exception("There are too many Hiding spot areas.");
            //else if (hidingSpotAreas.Length == 0)
            //    Debug.Log("Therer are np hiding spot areas detected");

            if (hidingSpots.Length > 0)
            {
                //add 5 nearest hiding spots
                Dictionary<HidingSpot, bool> excludedHidingSpots = new Dictionary<HidingSpot, bool>();
                for (int i = 0; i < 5; i++)
                {
                    //if there are fever hiding spots - quit
                    if (i >= hidingSpots.Length)
                        break;

                    HidingSpot chosenHidingSpotVar = hidingSpots[0];

                    foreach (HidingSpot hidingSpot in hidingSpots)
                    {
                        if (!excludedHidingSpots.ContainsKey(hidingSpot))
                            if (Vector3.Distance(hidingSpot.transform.position, transform.position) < Vector3.Distance(chosenHidingSpotVar.transform.position, transform.position))
                                chosenHidingSpotVar = hidingSpot;
                    }

                    Vector3 difference_hidingSpot = (GetGamesTransformPosition(GetGamesTransformPosition(chosenHidingSpotVar.transform.position)) - GetGamesTransformPosition(transform.position)).normalized;

                    sensor.AddObservation(difference_hidingSpot.x);
                    sensor.AddObservation(difference_hidingSpot.z);


                    excludedHidingSpots[chosenHidingSpotVar] = true;
                }

                //add information whether player is hidden
                bool playerIsHidden = GetComponentInParent<Game>().GetPlayer().GetComponent<DetectingPlayerInHidingSpot>().IsPlayerHidden();
                sensor.AddObservation(playerIsHidden);
            }

            
        }

        

    }

    void ResetDiamonds()
    {
        GetComponentInParent<Game>().ResetDiamonds();

        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            allDiamonds[diamond].ResetProperties();

        if (!spectating)
        {
            for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
                allDiamonds[diamond].ResetPosition();
        }

        NavMeshUpdate();
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

    void DistanceToDiamondRewardUpdate()
    {
        allCurrentDiamonds = GetComponentInParent<Game>().GetDiamonds();

        if (allCurrentDiamonds.Length > 0)
        {
            if (chosenDiamond == null)
                chosenDiamond = allCurrentDiamonds[0];

            //reward for making the distance to diamond smaller
            float newDistanceToDiamond = Vector3.Distance(chosenDiamond.transform.position, transform.position);
            if (allDistancesToDiamond.Count == 0)
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
    }

    void DistanceToClosestNPCRewardUpdate()
    {
        NPCmovement = GetComponentInParent<Game>().GetNPCmovements();
        if (NPCmovement.Length > 0)
        {
            //choose the closest NPC
            NPCMovement currentlyChosenNPC = chosenNPC;
            chosenNPC = NPCmovement[0];
            foreach (NPCMovement npc in NPCmovement)
            {
                if (Vector3.Distance(npc.transform.position, transform.position) < Vector3.Distance(chosenNPC.transform.position, transform.position))
                    chosenNPC = npc;
            }

            if (chosenNPC != currentlyChosenNPC)
                allDistancesToNPC = new List<float>();


            //reward for making the distance to diamond smaller
            float newDistanceToNPC = Vector3.Distance(chosenNPC.transform.position, transform.position);
            if (allDistancesToNPC.Count == 0)
                allDistancesToNPC.Add(newDistanceToNPC);

            if (newDistanceToNPC < allDistancesToNPC.Min())
            {
                allDistancesToNPC.Add(newDistanceToNPC);

                SetReward(+distanceToClosestNPCReward);
                minDistanceToNPC = newDistanceToNPC;
            }
        }
    }

    void DistanceToClosestHidingSpotUpdate()
    {
        hidingSpots = GetComponentInParent<Game>().GetHidingSpots();

        if (hidingSpots.Length > 0)
        {
            //choose the closest hiding spot
            HidingSpot currentlyChosenHidingSpot = chosenHidingSpot;
            chosenHidingSpot = hidingSpots[0];

            foreach (HidingSpot hidingSpot in hidingSpots)
            {
                //exclude hidingspots distance to which is smaller than 2
                if(Vector3.Distance(hidingSpot.transform.position, transform.position) >= 2)
                    if (Vector3.Distance(hidingSpot.transform.position, transform.position) < Vector3.Distance(chosenHidingSpot.transform.position, transform.position))
                        chosenHidingSpot = hidingSpot;
            }

            if (chosenHidingSpot != currentlyChosenHidingSpot)
                allDistancesToHidingSpot = new List<float>();


            //reward for making the distance to HidingSpot smaller
            float newDistanceHidingSpot = Vector3.Distance(chosenHidingSpot.transform.position, transform.position);
            if (allDistancesToHidingSpot.Count == 0)
                allDistancesToHidingSpot.Add(newDistanceHidingSpot);

            if (newDistanceHidingSpot < allDistancesToHidingSpot.Min())
            {
                allDistancesToHidingSpot.Add(newDistanceHidingSpot);

                SetReward(+distanceToClosestHidingSpotReward);
                minDistanceToHidingSpot = newDistanceHidingSpot;
            }
        }
    }

    void MaxStepsReachedRewardUpdate()
    {
        //if the reward wasn't collected during the episode, reset it
        stepsAfterReward++;
        if (stepsAfterReward == maxStepsAfterReward)
        {
            SetReward(+reachingMaxStepReward);
            stepsAfterReward = 0;
            ResetDiamonds();
            if (isTrainingOn && floorMeshRender != null)
            {
                floorMeshRender.material = neutralMaterial;
                if(imageRender != null)
                    imageRender.color = neutralColor;
            }
            EndEpisode();
        }
        else if (stepsAfterReward >= maxStepsBeforePenalty)
        {
            SetReward(+reachingMaxStepBeforePenaltyReward);
        }

        SetReward(+eachStepTakenReward);
    }

    void CheckIfPlayerHiddenUpdate()
    {
        bool playerIsHidden = GetComponentInParent<Game>().GetPlayer().GetComponent<DetectingPlayerInHidingSpot>().IsPlayerHidden();
        if (playerIsHidden)
        {
            SetReward(+playerIsHiddenReward);
        }
    }

    void CheckIfPlayerIsHiddenAndWithinNPCSFov()
    {
        if (GetComponentInParent<Game>().IsPlayerWithinNPCFOV() && GetComponentInParent<Game>().GetPlayer().GetComponent<DetectingPlayerInHidingSpot>().IsPlayerHidden())
        {
            SetReward(+playerIsHiddenAndWithinNPCFOVReward);
        }
    }

    void NavMeshUpdateWaypointIndex()
    {

        if (currentWaypointIndex < pathCorners.Count-1)
        {
            
            // Check if the agent is close enough to the waypoint
            if (Vector3.Distance(transform.position, pathCorners[currentWaypointIndex]) < 2f)
            {
                currentWaypointIndex++; // Move to the next waypoint
                allDistancesToWaypoint = new List<float>();
                SetReward(+nextNavMeshWaypointReached);
            }
        }

        //reward for making the distance to waypoint smaller
        float newDistanceToWaypoint = Vector3.Distance(pathCorners[currentWaypointIndex], transform.position);
        if (allDistancesToWaypoint.Count == 0)
            allDistancesToWaypoint.Add(newDistanceToWaypoint);

        if (newDistanceToWaypoint < allDistancesToWaypoint.Min())
        {
            allDistancesToWaypoint.Add(newDistanceToWaypoint);

            SetReward(+waypointDistanceReward);
            //Debug.Log("waypointDistanceReward " + newDistanceToWaypoint);
            minDistanceToWaypoint = newDistanceToWaypoint;
        }
        else if (newDistanceToWaypoint > allDistancesToWaypoint.Min())
            SetReward(+waypointInreasingDistanceReward);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {

        //in case sth wasn't loaded
        if (pathCorners.Count == 0)
        {
            Start();
        }

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool jump = actions.ContinuousActions[2] >=-1.0f && actions.ContinuousActions[2] <-0.5f? true : false;
        bool sneak = actions.ContinuousActions[2] >= -0.5f && actions.ContinuousActions[2] < 0.0f ? true : false;
        bool sprint = actions.ContinuousActions[2] >= 0.0f && actions.ContinuousActions[2] < 0.5f ? true : false;

        if (!isPauseOn)
        {
            //Debug.Log("Discrete action: " + actions.ContinuousActions[2]);

            //simply apply movement from ThirdPersonMovement
            GetComponent<ThirdPersonMovement>().ApplyMovement(moveX, moveZ, jump, sprint, sneak, false, 1f, false);

            //getting closer to diamond
            if (isTrainingOn)
            {
                DistanceToDiamondRewardUpdate();
                DistanceToClosestNPCRewardUpdate();
                DistanceToClosestHidingSpotUpdate();
                CheckIfPlayerHiddenUpdate();
                CheckIfPlayerIsHiddenAndWithinNPCSFov();
                NavMeshUpdateWaypointIndex();
                
            }

            MaxStepsReachedRewardUpdate();

            //check velocity
            CheckVelocity();
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

        SetReward(+reachingDiamondReward);
        //if (isTrainingOn && floorMeshRender != null)
        //{
        //    floorMeshRender.material = winMaterial;
        //}
        allDistancesToDiamond = new List<float>();
        stepsAfterReward = 0;

        NavMeshUpdate();

    }

    public void PlayerHasWon()
    {
        SetReward(+winningGameReward);
        Debug.Log("win " + GetCumulativeReward());
        if (isTrainingOn && floorMeshRender != null)
        {
            floorMeshRender.material = winMaterial;
            if (imageRender != null)
            {
                imageRender.color = winColor;
                //update UI text
                //if(GetComponentInParent<Game>().GetComponentInChildren<AchievedTimeUI>())
                //    GetComponentInParent<Game>().GetComponentInChildren<AchievedTimeUI>().UpdateAchievedTimeText();
            }
        }

        //update the round in metric script
        GetComponent<Metrics>().UpdateTheround();

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
        Debug.Log("complete loss " + GetCumulativeReward());

        if (isTrainingOn && floorMeshRender != null)
        {
            floorMeshRender.material = loseMaterial;
            if (imageRender != null)
                imageRender.color = loseColor;
        }

        //update the round in metric script
        GetComponent<Metrics>().UpdateTheround();

        EndEpisode() ;
    }

    public void PlayerDetectionCheck(bool canNPCSeePlayer)
    {
        if (canNPCSeePlayer)
        {
            SetReward(+NPCseePlayerReward);
        }
    }

    public void PlayerIsChasenAfter()
    {
        SetReward(+playerIschasedAfterReward);

        //update the round in metric script
        GetComponent<Metrics>().UpdateTheround();

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
        if (!spectating)
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
                    if (imageRender != null)
                        imageRender.color = loseColor;
                }

                //Debug.Log("barrier hit");
                Debug.Log("loss " + GetCumulativeReward());
                EndEpisode();

            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ////colliding with "what is barrier"
        if (other.gameObject.layer == whatIsBarrierLayer)
        {

            //obstacle was hit
            //SetReward(+hittingObstacleReward);
            //ResetDiamonds();
            //if (isTrainingOn && floorMeshRender != null)
            //{
            //    floorMeshRender.material = loseMaterial;
            //}
            //Debug.Log("trigger obstacle hit");
            //EndEpisode();

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
            
        }

    }

    void SetInitialValuesLineRenderer()
    {
        if(lineRenderers == null)
            //related to line renderer
            lineRenderers = new List<GameObject>();

        GameObject lineObj;
        lineObj = new GameObject("LineSegment"+ lineRenderers.Count);

        LineRenderer lineRendererVar = lineObj.AddComponent<LineRenderer>();

        lineRenderers.Add(lineObj);
        lineRenderer = lineRendererVar;


        if (lineRenderer)
        {
            lineRenderer.startWidth = 0.15f;
            lineRenderer.endWidth = 0.15f;
            if (enableLineRendering)
                lineRenderer.positionCount = 2;
            else
                lineRenderer.positionCount = 0;
            currentLine = 0;
            stopLineRendering = false;
            if (enableLineRendering)
            {

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position);
                currentLine += 2;
            }
        }
        
    }

    void UpdateLineRendererPath()
    {
        if (enableLineRendering)
        {
            lineRendererTimer += Time.deltaTime;

            if (lineRendererTimer >= 1f)
            {

                lineRenderer.positionCount++;

                //reset the timer
                lineRendererTimer = 0f;

                lineRenderer.SetPosition(currentLine, transform.position);
                previousLineRendererPosition = transform.position;
                //Debug.Log("Line between: " + currentLine + " and " + transform.position);

                currentLine++;
            }
        }
    }

    void BeforePositionSwitchNewEpisodeLineRenderer()
    {
        if (enableLineRendering)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(currentLine, transform.position);
            previousLineRendererPosition = transform.position;
            currentLine++;
        }
    }
    void StartNewLineRendererPath()
    {
        if (enableLineRendering && lineRenderer.positionCount > 2)
        {
            lineRenderer.positionCount++;
            //invisible break
            lineRenderer.SetPosition(currentLine, previousLineRendererPosition);
            currentLine++;
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

        if(!stopLineRendering && GetComponent<LineRenderer>())
                UpdateLineRendererPath();

    }
}
