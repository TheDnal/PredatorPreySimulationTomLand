using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    public static PlantSpawner instance;
    public GameObject plantPrefab;
    public int maxPlantCount = 10;
    private int plantCount = 0;
    private MapGenerator mapGen;
    private int iter = 0;
    void Awake()
    {
        if(instance != null )
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
    }
    public void Initialise()
    {
        mapGen = MapGenerator.instance;
    }
    void Update()
    {
        if(mapGen != null)
        {
            if(plantCount < maxPlantCount)
            {
                //Generate new plant
                SpawnPlant();
                plantCount++;
            }
        }
        
    }
    public void SpawnPlant()
    {
        iter++;
        //Get plant spawn location
        List<Vector3> spawns = mapGen.GetValidSpawnZones();
        int index = Random.Range(0, spawns.Count -1);
        float x = Random.Range(-4,4);
        float z = Random.Range(-4,4);
        Vector3 position = new Vector3(x/10, 0.6f,z/10);
        position += spawns[index];
        GameObject newPlant = Instantiate(plantPrefab, position, Quaternion.identity);
        newPlant.transform.parent = this.transform;
        newPlant.name = "Plant #" + iter;
        PartitionSystem.instance.AddGameObjectToPartition(newPlant, PartitionSystem.ObjectType.food);
        Vector2Int p = PartitionSystem.instance.WorldToPartitionCoords(newPlant.transform.position);
        newPlant.GetComponent<Plant>().setPartitionCoords(p);
    }
    public Vector3 getValidSpawn()
    {
        List<Vector3> spawns = mapGen.GetValidSpawnZones();
        int index = Random.Range(0,spawns.Count - 1);
        float x = Random.Range(-4,4);
        float z = Random.Range(-4,4);
        Vector3 pos = new Vector3(x/10, 0.6f,z/10);
        pos += spawns[index];
        return pos;
    }
    public void DecrementPlantCount()
    {
        plantCount --;
    }
}
