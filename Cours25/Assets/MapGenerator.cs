using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public enum GenerationType
    {
        RANDOM,
        PERLINNOISE
    }

    public GenerationType generationType;
    public int MapWidth;
    public int MapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;

    public TerrainType[] Ground;
    public TerrainType[] Node;

    public void GenerateMap()
    {
        if(generationType == GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }
        else if(generationType == GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }
    }

    public void GenerateMapWithNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapWidth, MapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapNode = Noise.GenerateNoiseMap(MapWidth, MapHeight, seed + 1, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseGisement = Noise.GenerateNoiseMap(MapWidth, MapHeight, seed + 3, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] customTilemap = new TileBase[MapWidth * MapHeight];
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float rnd = noiseMap[x, y];
                float NodeRnd = noiseMapNode[x, y];
                float gisement = noiseGisement[x, y];
                if (gisement > 0.7)
                {
                    customTilemap[y * MapWidth + x] = FindTileFromNode(NodeRnd);
                   /* if (NodeRnd > 0.6)
                    {
                       
                    }
                    else
                    {
                        customTilemap[y * MapWidth + x] = FindTileFromRegion(rnd);
                    }*/
                }
                else
                {
                    customTilemap[y * MapWidth + x] = FindTileFromRegion(rnd);
                }                
            }
        }
        SetTileMap(customTilemap);
    }

    public void GenerateMapWithRandom()
    {
        TileBase[] customTilemap = new TileBase[MapWidth * MapHeight];
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customTilemap[y * MapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTilemap);
    }
    
    private void SetTileMap(TileBase[] customTilemap)
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTilemap[y * MapWidth + x]);
            }
        }
    }

    private TileBase FindTileFromRegion(float rnd)
    {
        for (int x = 0; x < Ground.Length; x++)
        {
            if(rnd <= Ground[x].height)
            {
                return Ground[x].tile;
            }
        }
        return Ground[0].tile;
    }

    private TileBase FindTileFromNode(float rnd)
    {
        for (int x = 0; x < Node.Length; x++)
        {
            if (rnd <= Node[x].height)
            {
                return Node[x].tile;
            }
        }
        return Node[0].tile;
    }

    private void OnValidate()
    {
        if (MapHeight < 1)
            MapHeight = 1;
        if (MapWidth < 1)
            MapWidth = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}
