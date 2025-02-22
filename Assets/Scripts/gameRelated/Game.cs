using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Tile;
using System.Linq;

public class Game : MonoBehaviour
{
    Diamond[] allDiamonds;
    public NPCMovement[] NPCmovement;
    public ThirdPersonMovement player;
    public SpawningLocation[] spawningLocations;
    public PlayerSpawnPoint[] playerSpawnPoints;
    public Barrier[] barriers;
    public Tile[] tiles;
    public HidingSpotArea[] hidingSpotAreas;
    public HidingSpot[] hidingSpots;
    Tile currentlyTouchedTile;

    [Header("Minimap")]
    public GameObject minimap;
    public bool minimapIsOn = false;

    [Header("Pause")]
    public GameObject pauseMenu;
    public bool pauseMenuIsoOn = false;


    [Header("End of the game")]
    public GameObject endGameMenu;
    

    [Header("Diamonds")]
    public int collectedDiamonds;
    public int allDiamondsNumber;

    [Header("For Additional Training")]
    public bool isTrainingOn;
    public int testNr;

    [Header("Djikstra path finding")]
    private List<Tile> djikstraPath;
    //public bool enableDjikstraPathFinding;
    private TileType[] excludedTypes;
    List<Tile> diamondTiles;
    private Material pathMaterial;
    private float radiusForFindingObjects;


    [Header("Layers")]
    public LayerMask playerMask;
    public LayerMask diamondMask;

    [Header("Camera")]
    public MainCamera cam;

    public HidingSpotArea[] GetHidingSpotAreas()
    {
        hidingSpotAreas = GetComponentsInChildren<HidingSpotArea>();
        return hidingSpotAreas;
    }

    public HidingSpot[] GetHidingSpots()
    {
        hidingSpots = GetComponentsInChildren<HidingSpot>();
        return hidingSpots;
    }
    public GameObject GetEndGameMenu()
    {
        return endGameMenu;
    }
    public bool GetIsPauseMenuOn()
    {
        return pauseMenuIsoOn;
    }
    public MainCamera GetCamera()
    {
        return cam;
    }
    private List<Tile> GetPredecessorsDjikstra()
    {
        Djikstra djikstra = new Djikstra();
        tiles = GetTiles();

        if (radiusForFindingObjects == 0)
        {
            radiusForFindingObjects = 2;

        }

        List<Tile> playerTiles = new List<Tile>();
        playerTiles = djikstra.FindTilesWithObjects(tiles, GetPlayer().gameObject, radiusForFindingObjects);
        Tile playerTile = new Tile();
        if (playerTiles.Count > 0)
            playerTile = playerTiles[0];
        else
        {
            Debug.Log("Player tile wasn't found");
            playerTiles = djikstra.FindTilesWithObjects(tiles, GetPlayer().gameObject, radiusForFindingObjects + 0.2f);

            if (playerTiles.Count > 0)
                playerTile = playerTiles[0];
            else
                Debug.Log("Player tile wasn't found 2" );
        }

        diamondTiles = new List<Tile>();
        allDiamonds = GetDiamonds();
        foreach(Diamond diamond in allDiamonds)
        {
            List<Tile> diamondTilesVar = djikstra.FindTilesWithObjects(tiles, diamond.gameObject, radiusForFindingObjects);
            if (diamondTilesVar.Count > 0)
                diamondTiles.Add(diamondTilesVar[0]);
            else
                Debug.Log("Didn't find diamond " + diamond);
        }
        if (diamondTiles.Count == 0)
            Debug.Log("Diamond tiles weren't found");

        List<Tile> predecessors = new List<Tile>();
        //assign type to each tile beforehand
        foreach (Tile tile in tiles) tile.AssignType();

        predecessors = djikstra.DjikstraMethod(tiles, playerTile, excludedTypes);

        return predecessors;
    }

    private List<List<Tile>> GetDjikstraPaths(List<Tile> predecessors)
    {
        List<List<Tile>> djikstraPathsToDiamonds = new List<List<Tile>>();
        Diamond[] diamonds = GetDiamonds();

        //create path to each diamond
        foreach (Tile diamondTile in diamondTiles)
        {
            List<Tile> path = new List<Tile>();

            Tile currentlyCheckedTile = diamondTile;
            for (int i = 0; i < 1000; i++)
            {
                if (currentlyCheckedTile == transform.GetComponent<Tile>() || currentlyCheckedTile == predecessors[currentlyCheckedTile.index])
                    break;

                path.Add(currentlyCheckedTile);
                //update the color
                //if(currentlyCheckedTile.GetComponentInChildren<MeshRenderer>())
                //    currentlyCheckedTile.GetComponentInChildren<MeshRenderer>().material = pathMaterial;

                currentlyCheckedTile = predecessors[currentlyCheckedTile.index];

            }

            //reverse the list
            path.Reverse();

            //add the list to tthe tilesWithDiamonds
            djikstraPathsToDiamonds.Add(path);

        }

        return djikstraPathsToDiamonds;
    }
    public List<Tile> CreateDjikstraPath()
    {
        djikstraPath = new List<Tile>();
        List<Tile> predecessors = new List<Tile>();
        predecessors = GetPredecessorsDjikstra();
        List<List<Tile>> djikstraPathsToDiamonds = GetDjikstraPaths(predecessors);

        if (djikstraPathsToDiamonds.Count > 0)
        {
            //we choose djikstra path with diamond to which there is the smallest distance to
            List<Tile> minDistanceToDiamondPath = djikstraPathsToDiamonds[0];
            if (minDistanceToDiamondPath.Count > 0)
            {
                foreach (List<Tile> path in djikstraPathsToDiamonds)
                {
                    if (path.Count > 0)
                    {
                        float checkedDistance = Vector3.Distance(path.Last().transform.position, GetPlayer().transform.position);
                        float currentDistance = Vector3.Distance(minDistanceToDiamondPath.Last().transform.position, GetPlayer().transform.position);

                        if (checkedDistance < currentDistance)
                            minDistanceToDiamondPath = path;
                    }
                }
            }

            djikstraPath = minDistanceToDiamondPath;
        }
        else
            Debug.Log("Djikstra path wasn't found");

        return djikstraPath;
    }

    public Tile GetNextTileToGoTo()
    {
        Tile nextTileToGoTo = new Tile();

        CreateDjikstraPath();
        if (djikstraPath.Count > 0)
            nextTileToGoTo = djikstraPath[0];
        //else
        //    Debug.Log("Djikstra path wasn't found");

        return nextTileToGoTo;
    }

    public void UpdateDjikstraPath(List<Tile> djikstraPathVar)
    {
        djikstraPath = new List<Tile>();
        djikstraPath = djikstraPathVar;
    }
    public bool GetIsTrainingOn()
    {
        return isTrainingOn;
    }

    public int GetTestNr()
    {
        return testNr;
    }

    // Start is called before the first frame update
    void Start()
    {
        allDiamonds = GetComponentsInChildren<Diamond>();
        NPCmovement = GetComponentsInChildren<NPCMovement>();

        allDiamondsNumber = allDiamonds.Length;

        //lock the cursor upon loading the game
        if(!(isTrainingOn && !GetPlayer().GetComponent<MLPlayerAgent>().spectating))
            LockCursor.Instance.LockTheCursor();

        //in case the time is frozen, unfreeze it
        Time.timeScale = 1f;
    }

    public void ResetDiamonds()
    {
        collectedDiamonds = 0;
        GetComponentInChildren<PlayerInventory>().ResetProperties();
    }
    public void UpdateCollectedDiamonds()
    {
        collectedDiamonds ++;

        if(collectedDiamonds == allDiamondsNumber)
        {
            GetComponent<GameOver>().UpdateGameOver(true);
            GetComponent<GameOver>().UpdatePlayerWon(true);
        }
    }

    public void UpdateCurrentlyTouchedTile(Tile currentlyTouchedTileVar)
    {
        currentlyTouchedTile = currentlyTouchedTileVar;
    }

    public Tile GetCurrentlyTouchedTile()
    {
        return currentlyTouchedTile;
    }

    public Tile[] GetTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
        return tiles;
    } 
    public Barrier[] GetBarriers()
    {
        barriers = GetComponentsInChildren<Barrier>();
        return barriers;
    }

    public SpawningLocation[] GetSpawningLocations()
    {
        spawningLocations = GetComponentsInChildren<SpawningLocation>();
        return spawningLocations;
    }

    public PlayerSpawnPoint[] GetPlayerSpawningPoints()
    {
        playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>();
        return playerSpawnPoints;
    }
    public Diamond[] GetDiamonds()
    {
        Diamond[] allDiamondsVariable = GetComponentsInChildren<Diamond>();

        return allDiamondsVariable;
    }

    public ThirdPersonMovement GetPlayer()
    {
        player = GetComponentInChildren<ThirdPersonMovement>();
        return player;
    }


    public NPCMovement[] GetNPCmovements()
    {
        NPCMovement[] NPCmovementVariable = GetComponentsInChildren<NPCMovement>();
        return NPCmovementVariable;
    }

    public void ResetPauseMenu()
    {
        pauseMenuIsoOn = true;
        UpdatePauseMenu();
    }
    void UpdatePauseMenu()
    {
        
            if (pauseMenuIsoOn)
            {
                pauseMenuIsoOn = false;
                //locking the cursor during the gameplay
                if(!(isTrainingOn && !GetPlayer().GetComponent<MLPlayerAgent>().spectating))
                    LockCursor.Instance.LockTheCursor();
            }
            else
            {
                pauseMenuIsoOn = true;
                //unlocking the cursor in the menu
                if (!(isTrainingOn && !GetPlayer().GetComponent<MLPlayerAgent>().spectating))
                    LockCursor.Instance.UnlockTheCursor();
            }
        pauseMenu.GetComponentInChildren<PauseMenu>().UpdatePauseMenu(pauseMenuIsoOn);

        

    }

    void UpdateMiniMapUI()
    {

        if (!(isTrainingOn && !GetPlayer().GetComponent<MLPlayerAgent>().spectating))
        {
            if (Input.GetKey(KeyCode.M))
            {
                minimapIsOn = true;
                minimap.SetActive(true);
            }
            else
            {
                minimapIsOn = false;
                minimap.SetActive(false);
            }
        }
        



    }

    public bool IsPlayerWithinNPCFOV()
    {
        NPCFieldOfView[] NPCfovs = GetComponentsInChildren<NPCFieldOfView>();

        foreach(NPCFieldOfView npc in NPCfovs)
            if (npc.IsPlayerWithinFOV())
                return true;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetIsTrainingOn() || GetPlayer().GetComponent<MLPlayerAgent>().spectating)
        {
            
            UpdateMiniMapUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            UpdatePauseMenu();
    }
}
