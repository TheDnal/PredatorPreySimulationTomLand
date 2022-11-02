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
        //Get plant spawn location
        List<Vector3> spawns = mapGen.GetValidSpawnZones();
        int index = Random.Range(0, spawns.Count -1);
        float x = Random.Range(-4,4);
        float z = Random.Range(-4,4);
        Vector3 position = new Vector3(x/10, 0.6f,z/10);
        position += spawns[index];
        GameObject newPlant = Instantiate(plantPrefab, position, Quaternion.identity);
        newPlant.transform.parent = this.transform;
        PartitionSystem.instance.AddGameObjectToPartition(newPlant, PartitionSystem.ObjectType.food);
        Vector2Int p = PartitionSystem.instance.GetPartitionCoords(newPlant.transform.position);
        PartitionSystem.instance.partitions[p.x,p.y].IncrementFoodCount();
    }
    public void DecrementPlantCount()
    {
        plantCount --;
    }
}
