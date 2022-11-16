using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    /*
    - Tiles are generated from a perlin noise map
    - Tile color follow a gradient, with sand being the lowest, then grass, then stone than snow.
    - World Sea level defaults at height 0.4. Any tiles that have a height lower than this value have their
    color overriden by a water gradient, with light blue being shallow water and deep blue being deep water.
    - The player can dynamically change a Sea level offset value, being a scale of -1 to 1, with minus
    value representing drought and positive values representing a flood.
    - This offset calculates a current sea level, equalling to the World sea level + offset. If the sea level lowers,
    tiles below the world sea level but above the current sea level turn to sand, and other water tiles have their
    color adjusted to represent their shallowing. 
    If the sea level rises, if any tile is below the current sea level it turns to water, with its color(depth) being
    the difference between the current sea level and the tiles height.
    If a flood recedes, land that was flooded will return to its default color (sand/grass/stone etc). 
    */

    public static MapGenerator instance;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    //World sea level
    [Range(0,1)]
    public float WorldSeaLevel = 0.4f;
    [Range(-1,1)]
    public float seaLevelOffset = 0;
    public float heightOffset = 0;
    [Space(6) ,Header("Prefabs")]
    public GameObject LandTilePrefab;
    public GameObject mapBoundryPrefab;
    public GameObject OceanTilePrefab;
    [Space(6)]
    private float[,] noiseMap;
    public Gradient LandColorGradient;
    public Gradient WaterColorGradient;
    public Color DriedOceanColor;
    private List<GameObject> Tiles = new List<GameObject>();
    private GameObject[,] TileArray;
    public void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
        GenerateMap();
        GenerateTiles();
        UpdateSeaLevel();
    }
    void Start()
    {
        Vector2Int mapDimensions = new Vector2Int(mapWidth, mapHeight);
        Vector3 mapOrigin = new Vector3(-mapWidth/2, 0, -mapHeight/2);
        PartitionSystem.instance.Initialise(mapOrigin, mapDimensions, 1);
        PlantSpawner.instance.Initialise();
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
        TileArray = new GameObject[mapWidth,mapHeight];
        for(int i =0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                float height = noiseMap[i,j];
                Vector3 pos = new Vector3(i,0,j);
                pos -= new Vector3(mapWidth/2, 0, mapHeight/2);
                Color col = Color.white;
                prefab = LandTilePrefab;
                if(height > WorldSeaLevel)
                {
                    float colHeight = height/ 1-WorldSeaLevel;

                    col = LandColorGradient.Evaluate(colHeight);
                    if(height >= 0.8f)
                    {
                        float offset = height - 0.8f;
                        pos.y = height + offset;
                    }
                }
                else
                {
                    float waterDepth = WorldSeaLevel - height;
                    col = WaterColorGradient.Evaluate(waterDepth);
                    prefab = OceanTilePrefab;
                    pos.y = -WorldSeaLevel;
                }
                GameObject newTile = Instantiate(prefab, pos, Quaternion.identity);
                newTile.GetComponent<MeshRenderer>().material.color = col;
                newTile.transform.parent = this.transform;
                TileArray[i,j] = newTile;
            }
        }
        //Generate North/South Wall
        for(int i = -1; i < mapWidth + 1; i++)
        {
            float height = 0f;
            Vector3 pos = new Vector3(i,2.5f,-1) - new Vector3(mapWidth/2, 0, mapHeight/2);
            Color col = LandColorGradient.Evaluate(1);
            pos.y += height;
            GameObject southWallTile = Instantiate(mapBoundryPrefab, pos, Quaternion.identity);
            southWallTile.GetComponent<MeshRenderer>().material.color = col;
            southWallTile.transform.parent = this.transform;
            
            pos = new Vector3(i,2.5f, mapHeight) - new Vector3(mapWidth/2, 0, mapHeight/2);
            pos.y += height;
            GameObject northWallTile = Instantiate(mapBoundryPrefab, pos, Quaternion.identity);
            northWallTile.GetComponent<MeshRenderer>().material.color = col;
            northWallTile.transform.parent = this.transform;
        }
        for(int j = -1; j < mapHeight; j++)
        {
            float height = 0f;
            Vector3 pos = new Vector3(-1,2.5f,j) - new Vector3(mapWidth/2, 0, mapHeight/2);
            Color col = LandColorGradient.Evaluate(1);
            pos.y += height;
            GameObject WestWallTile = Instantiate(mapBoundryPrefab, pos, Quaternion.identity);
            WestWallTile.GetComponent<MeshRenderer>().material.color = col;
            WestWallTile.transform.parent = this.transform;

            pos = new Vector3(mapWidth, 2.5f, j) - new Vector3(mapWidth/2, 0, mapHeight/2);
            pos.y += height;
            GameObject EastWallTile = Instantiate(mapBoundryPrefab, pos, Quaternion.identity);
            EastWallTile.GetComponent<MeshRenderer>().material.color = col;
            EastWallTile.transform.parent = this.transform;
        }
    }
    public List<Vector3> GetValidSpawnZones()
    {
        List<Vector3> validSpawns = new List<Vector3>();
        foreach(GameObject tile in TileArray)
        {
            if(tile.layer == 6 && tile.transform.position.y < 0.8f)
            {
                validSpawns.Add(tile.transform.position);
            }
        }   
        return validSpawns;
    }
    public bool isTileUnderWater(Vector2Int tileCoord)
    {
        float height = noiseMap[tileCoord.x,tileCoord.y];
        return height > WorldSeaLevel ? false : true;
    }
    public bool isTileTraversable(Vector2Int tileCoord)
    {
        float height = noiseMap[tileCoord.x, tileCoord.y];
        if(height < WorldSeaLevel || height >= 0.80f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //Will adjust all the tiles in the map to accommodate the new sea level
    public void UpdateSeaLevel()
    {
        //No map
        if(TileArray == null)
        {
            return;
        }

        float currentSeaLevel = WorldSeaLevel + seaLevelOffset;
        //Iterate over every tile
        for(int i =0 ; i < mapWidth; i ++){
            for(int j = 0; j < mapHeight; j ++)
            {
                
                //Get relevant references
                float tileHeight = noiseMap[i,j];
                GameObject currTile = TileArray[i,j];
                bool isBelowSeaLevel = tileHeight < WorldSeaLevel ? true : false;
                Vector3 pos = currTile.transform.position;
                Color tileColor;
                //Tile is flooded
                if(tileHeight < currentSeaLevel)
                {

                    //Tile aligns with sea level
                    pos.y = -WorldSeaLevel;
                    //Tiles colour is adjusted to represent water depth
                    float waterDepth = currentSeaLevel - tileHeight;
                    tileColor = WaterColorGradient.Evaluate(waterDepth);
                    currTile.GetComponent<MeshRenderer>().material.color = tileColor;
                    currTile.transform.position = pos;
                    
                }
                //Tile experiences Drought
                else
                {
                    //Tile turns from water to sand
                    if(isBelowSeaLevel)
                    {
                        pos.y = -WorldSeaLevel;
                        currTile.GetComponent<MeshRenderer>().material.color = DriedOceanColor;
                        currTile.transform.position = pos;
                    }
                    //Tile turns from water to land
                    else
                    {
                        pos.y = 0;
                        if(tileHeight >= 0.8f)
                        {
                            pos.y = tileHeight;
                        }
                        tileColor = LandColorGradient.Evaluate(tileHeight/ 1-WorldSeaLevel);
                        currTile.GetComponent<MeshRenderer>().material.color = tileColor;
                        currTile.transform.position = pos;  
                    }
                }
            }   
        }
    }

}
