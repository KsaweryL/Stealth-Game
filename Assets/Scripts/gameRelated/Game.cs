using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    Diamond[] allDiamonds;
    public NPCMovement[] NPCmovement;
    public ThirdPersonMovement player;
    public SpawningLocation[] spawningLocations;
    public Barrier[] barriers;

    [Header("Diamonds")]
    public int collectedDiamonds;
    public int allDiamondsNumber;

    [Header("For Additional Training")]
    public bool isTrainingOn;
    public int testNr;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
