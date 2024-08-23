using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Linq;

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
    public LayerMask[] layersToCollideWith; 

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

    private void AssignCoords()
    {
        x = GetGamesTransformPosition(transform.position).x;
        z = GetGamesTransformPosition(transform.position).z;
    }

    public void AssignType()
    {
        foreach (LayerMask layer in layersToCollideWith)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1, layer);

            if (hitColliders.Length > 0)
            {
                switch (hitColliders[0].gameObject.layer)
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
        }

    }

    public void ResetType()
    {
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

    }

    private void OnTriggerExit(Collider other)
    {
        type = TileType.Default;
        if(GetComponentInChildren<MeshRenderer>())
            GetComponentInChildren<MeshRenderer>().material = gridMaterial;
    }


}
