using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        NA,
        Water,
        UI,
        whatIsGround,
        RiverWalls,
        whatIsPlayer,
        whatIsBarrier,
        hidingSpot,
        NPC,
        Diamond,
        NotFullBarrier,
        water

    }

    public TileType type;

    private void AssignType(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 6: 
                type = TileType.whatIsGround;
                break;
            case 7:
                type = TileType.RiverWalls;
                break;
            case 8:
                type = TileType.whatIsPlayer;
                break;
            case 9:
                type = TileType.whatIsBarrier;
                break;
            case 10:
                type = TileType.hidingSpot;
                break;
            case 11:
                type = TileType.NPC;
                break;
            case 12:
                type = TileType.Diamond;
                break;
            case 13:
                type = TileType.NotFullBarrier;
                break;
            case 14:
                type = TileType.water;
                break;
            default:
                type = TileType.Default;
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello");
        AssignType(other);

    }
}
