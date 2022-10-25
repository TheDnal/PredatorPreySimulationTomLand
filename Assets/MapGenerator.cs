using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float heightOffset = 0;
    public bool autoUpdate;

    [Space(6) ,Header("Prefabs")]
    public GameObject LandTilePrefab;
    public GameObject OceanTilePrefab;
    [Space(6)]
    private float[,] noiseMap;
    public Gradient heightColorKey;
    private List<GameObject> Tiles = new List<GameObject>();
    public void Awake()
    {
        GenerateMap();
        GenerateTiles();
    }
    public void GenerateMap()
    {
        //gets noisemap from noise class
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed ,  noiseScale, octaves, persistance, lacunarity, offset, heightOffset);
        
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

    void GenerateTiles()
    {
        GameObject prefab;
        for(int i =0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                float height = noiseMap[i,j];
                Vector3 pos = new Vector3(i,1,j);
                pos -= new Vector3(mapWidth/2, 0, mapHeight/2);
                Color col = heightColorKey.Evaluate(height);
                prefab = LandTilePrefab;
                if(height >= 0.8f)
                {
                    pos.y += height;
                }
                if(height <= 0.4f)
                {
                    prefab = OceanTilePrefab;
                    pos.y -= height;
                }
                GameObject newTile = Instantiate(prefab, pos, Quaternion.identity);
                newTile.GetComponent<MeshRenderer>().material.color = col;
                newTile.transform.parent = this.transform;
                Tiles.Add(newTile);
            }
        }
        //Generate North/South Wall
        for(int i = -1; i < mapWidth + 1; i++)
        {
            float height = 0f;
            Vector3 pos = new Vector3(i,1,-1) - new Vector3(mapWidth/2, 0, mapHeight/2);
            Color col = heightColorKey.Evaluate(height);
            prefab = OceanTilePrefab;
            pos.y += height;
            GameObject southWallTile = Instantiate(prefab, pos, Quaternion.identity);
            southWallTile.GetComponent<MeshRenderer>().material.color = col;
            southWallTile.transform.parent = this.transform;
            
            pos = new Vector3(i, 1, mapHeight) - new Vector3(mapWidth/2, 0, mapHeight/2);
            pos.y += height;
            GameObject northWallTile = Instantiate(prefab, pos, Quaternion.identity);
            northWallTile.GetComponent<MeshRenderer>().material.color = col;
            northWallTile.transform.parent = this.transform;
        }
        for(int j = -1; j < mapHeight; j++)
        {
            float height = 0f;
            Vector3 pos = new Vector3(-1,1,j) - new Vector3(mapWidth/2, 0, mapHeight/2);
            Color col = heightColorKey.Evaluate(height);
            prefab = OceanTilePrefab;
            pos.y += height;
            GameObject WestWallTile = Instantiate(prefab, pos, Quaternion.identity);
            WestWallTile.GetComponent<MeshRenderer>().material.color = col;
            WestWallTile.transform.parent = this.transform;

            pos = new Vector3(mapWidth, 1, j) - new Vector3(mapWidth/2, 0, mapHeight/2);
            pos.y += height;
            GameObject EastWallTile = Instantiate(prefab, pos, Quaternion.identity);
            EastWallTile.GetComponent<MeshRenderer>().material.color = col;
            EastWallTile.transform.parent = this.transform;
        }
    }
    public List<Vector3> GetValidSpawnZones()
    {
        List<Vector3> validSpawns = new List<Vector3>();
        foreach(GameObject tile in Tiles)
        {
            if(tile.layer == 6)
            {
                validSpawns.Add(tile.transform.position);
            }
        }   
        return validSpawns;
    }
    [System.Serializable]
    public struct TileType{
        public string tileName;
        public Color tileColor;
        public float height;
    }
}
