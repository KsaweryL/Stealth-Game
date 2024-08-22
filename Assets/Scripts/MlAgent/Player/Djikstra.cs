using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Tile;

public class Djikstra : MonoBehaviour
{
    public List<Tile> DjikstraMethod(Tile[] tiles, Tile startingTile, TileType[] excludedTypes)
    {
        List<Tile> predecessors = new List<Tile>();
        List<float> distances = new List<float>();

        //tile is a key, index in tile table is a value
        Dictionary<Tile, int> priorityQueue = new Dictionary<Tile, int>();
        Dictionary<Tile, int> tilesDict = new Dictionary<Tile, int>();


        int startingTileIndex = 0;

        for (int tile = 0; tile < tiles.Length; tile++)
        {
            distances.Add(100f);
            predecessors.Add(tiles[tile]);     //initially, ever Tile's predecessor is itself
            priorityQueue.Add(tiles[tile], tile);
            tilesDict.Add(tiles[tile], tile);

            //to find the starting tile
            if (tiles[tile] == startingTile)
                startingTileIndex = tile;

            //for debugging
            tiles[tile].index = tile;

            if (tiles[tile] == startingTile)
                startingTileIndex = tile;
        }

        //for the starting tile
        distances[startingTileIndex] = 0;
        predecessors[startingTileIndex] = startingTile;

        while (priorityQueue.Count > 0) {

            //get the distance value form the first tile in the queue
            int minDistanceIndex = priorityQueue.First().Value;
            Tile minDistanceTile = priorityQueue.First().Key;

            foreach (var pair in priorityQueue) {
                if (distances[pair.Value] < distances[minDistanceIndex])
                {
                    minDistanceIndex = pair.Value;
                    minDistanceTile = pair.Key;
                }
            }

            for(int neighbouringTile = 0;  neighbouringTile < minDistanceTile.neighbouringTiles.Count; neighbouringTile++)
            {
                Relaxation(tiles, distances, predecessors, minDistanceIndex, tilesDict[minDistanceTile.neighbouringTiles[neighbouringTile]], excludedTypes);
            }

            priorityQueue.Remove(minDistanceTile);
        
        }

        return predecessors;
    }

    private void Relaxation(Tile[] tiles, List<float> distances, List<Tile> predecessors, int tile1Index, int tile2Index, TileType[] excludedTypes)
    {
        float distance = 0;
        distance = Vector3.Distance(tiles[tile1Index].transform.position, tiles[tile2Index].transform.position);

        //if the second tile belong to the excluded types of tiles, then distance between them will be considered as big
        foreach (TileType type in excludedTypes)
        {
            if (tiles[tile1Index].type == type || tiles[tile2Index].type == type)
                distance = 1000f;
        }

        if (distances[tile1Index] + distance < distances[tile2Index])
        {
            distances[tile2Index] = distances[tile1Index] + distance;
            predecessors[tile2Index] = tiles[tile1Index];
        }
    }
}
