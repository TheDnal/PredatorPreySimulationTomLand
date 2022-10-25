using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public MapGenerator generator;
    public int startingPopulation;
    public GameObject entityPrefab;

    void Start()
    {
        List<Vector3> validSpawns = generator.GetValidSpawnZones();
        SpawnEntities(validSpawns);
    }

    void SpawnEntities(List<Vector3> spawnZones)
    {
        if(spawnZones.Count < startingPopulation)
        {
            startingPopulation = spawnZones.Count;
        }
        for(int i = 0; i < startingPopulation; i++ )
        {
            if(spawnZones.Count == 0)
            {
                break;
            }
            int index = Random.Range(0, spawnZones.Count -1 );
            Vector3 spawnPos = spawnZones[index];
            spawnPos += Vector3.up;
            GameObject newEntity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            newEntity.transform.parent = this.transform;
            spawnZones.Remove(spawnPos);
        }
    }
}
