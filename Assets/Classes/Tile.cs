using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Game game;
    public List<Tile> neighbouringTiles;
    public float radius;
    public LayerMask tileLayerMask;
    Diamond[] diamonds;
    bool touchingThePlayer;

    [Header("path related to djikstra method")]
    public List<Tile> predecessors;
    public List<List<Tile>> djikstraPathsToDiamonds;
    public List<Tile> tilesWithDiamonds;
    public List<Tile> path;
    public Material pathMaterial;
    public Material gridMaterial;

    public int index;

    [Header("Excluded layers")]
    //layers that are excluded from djikstra algorithm
    public TileType[] excludedTypes;

    [Header("Other")]


    Djikstra djikstra;
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

    public float x;
    public float z;

    public Vector3 GetGamesTransformPosition(Vector3 position)
    {
        game = GetComponentInParent<Game>();
        return game.transform.InverseTransformPoint(position);
    }

    private void AssignNeighbours()
    {
        neighbouringTiles = new List<Tile>();

        Vector3 mainTilePosition = transform.position;

        // Find all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(mainTilePosition, radius, tileLayerMask);

        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.transform.position != transform.position)
                neighbouringTiles.Add(hitCollider.gameObject.GetComponent<Tile>());
        }

    }

    public void CreateDjikstraPaths()
    {
        type = TileType.whatIsPlayer;

        GetPredecessors();

        djikstraPathsToDiamonds = new List<List<Tile>>();
        diamonds = game.GetDiamonds();

        //first, we will find all tiles that contain diamonds
        tilesWithDiamonds = new List<Tile>();

        foreach (Tile tile in predecessors)
        {
            if (tile.type == TileType.Diamond)
            {
                tilesWithDiamonds.Add(tile);
            }
        }

        path = new List<Tile>();

        //create path to each diamond
        foreach (Tile diamondTile in tilesWithDiamonds)
        {
            List<Tile> pathVariable = new List<Tile>();

            Tile currentlyCheckedTile = diamondTile;
            for (int i = 0; i < 1000; i++)
            {
                if (currentlyCheckedTile == transform.GetComponent<Tile>() || currentlyCheckedTile == predecessors[currentlyCheckedTile.index])
                    break;

                pathVariable.Add(currentlyCheckedTile);
                //update the color
                currentlyCheckedTile.GetComponentInChildren<MeshRenderer>().material = pathMaterial;

                currentlyCheckedTile = predecessors[currentlyCheckedTile.index];
                
            }

            //reverse the list
            pathVariable.Reverse();

            //add the list to tthe tilesWithDiamonds
            djikstraPathsToDiamonds.Add(pathVariable);

            //for debugging
            path = pathVariable;

        }

    }

    private void GetPredecessors() {
        predecessors = new List<Tile>();
        djikstra = new Djikstra();
        predecessors = djikstra.DjikstraMethod(game.GetTiles(), transform.GetComponent<Tile>(), excludedTypes);

    }

    private void AssignCoords()
    {
        x = GetGamesTransformPosition(transform.position).x;
        z = GetGamesTransformPosition(transform.position).z;
    }

    public void AssignType(Collider other)
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

    public void ResetType()
    {
        if(type == TileType.Diamond || type == TileType.whatIsPlayer)
            type = TileType.Default;
    }
    private void Start()
    {
        if (radius == 0)
            radius = 1.5f;
        AssignCoords();
        AssignNeighbours();
        touchingThePlayer = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        AssignType(other);
        //create new djikstra paths
        if (other.gameObject.layer == 8)
        {
            CreateDjikstraPaths();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        type = TileType.Default;
        GetComponentInChildren<MeshRenderer>().material = gridMaterial;
    }


}
