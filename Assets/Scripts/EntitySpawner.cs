using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    //AI Entity spawner Class. Gets a list of valid spawn points from the map generator and spawns AI on those spots
    public MapGenerator generator;
    public int startingPopulation;
    public GameObject entityPrefab;

    public static EntitySpawner instance;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this){Destroy(this);}
        }
        else{instance = this;}
    }
    void Start()
    {
        List<Vector3> validSpawns = generator.GetValidSpawnZones();
        SpawnEntities(validSpawns);
    }

    void SpawnEntities(List<Vector3> spawnZones)
    {
        for(int i = 0; i < startingPopulation; i++ )
        {
            //Ran out of spawnzones
            if(spawnZones.Count == 0)
            {
                Debug.Log("Error : Starting population exceeded valid map spawns.");
                Debug.Log("Entities attemped to spawn : " + startingPopulation);
                Debug.Log("Entities successfully spawned : " + i);
                break;
            }
            int index = Random.Range(0, spawnZones.Count -1 );
            Vector3 spawnPos = spawnZones[index];
            spawnPos += Vector3.up * 0.75f;
            GameObject newEntity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            newEntity.transform.parent = this.transform;
            spawnZones.Remove(spawnZones[index]);
        }
    }
}
