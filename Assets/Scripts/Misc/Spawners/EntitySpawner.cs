using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    //AI Entity spawner Class. Gets a list of valid spawn points from the map generator and spawns AI on those spots
    public MapGenerator generator;
    public int startingPreyPopulation, startingPredatorPopulation, currentPopulation;
    public GameObject preyPrefab, predatorPrefab;
    public Material MaleMat,FemaleMat;
    public static EntitySpawner instance;
    private List<GameObject> entities = new List<GameObject>();
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this){Destroy(this);}
        }
        else{instance = this;}
    }
    public void Initialise()
    {
        List<Vector3> validSpawns = PartitionSystem.instance.GetValidSpawnZones();
        SpawnPrey(validSpawns);
        SpawnPredators(validSpawns);
    }
    void SpawnPrey(List<Vector3> spawnZones)
    {
        for(int i = 0; i < startingPreyPopulation; i++ )
        {
            //Ran out of spawnzones
            if(spawnZones.Count == 0)
            {
                Debug.Log("Error : Starting population exceeded valid map spawns.");
                Debug.Log("Entities attemped to spawn : " + startingPreyPopulation);
                Debug.Log("Entities successfully spawned : " + i);
                break;
            }
            //Spawn entity
            int index = Random.Range(0, spawnZones.Count -1 );
            Vector3 spawnPos = spawnZones[index];
            spawnPos += Vector3.up * 0.75f;
            GameObject newEntity = Instantiate(preyPrefab, spawnPos, Quaternion.identity);

            //Set gender
            int gender = Random.Range(0,2);
            newEntity.GetComponent<PreyAgent>().Initialise(gender, GeneticsSystem.GetStartingPreyGenome());

            //Misc
            newEntity.transform.parent = this.transform;
            entities.Add(newEntity);
            spawnZones.Remove(spawnZones[index]);
            currentPopulation++;
        }
    }
    void SpawnPredators(List<Vector3> spawnZones)
    {
        for(int i = 0; i < startingPredatorPopulation; i++ )
        {
            //Ran out of spawnzones
            if(spawnZones.Count == 0)
            {
                Debug.Log("Error : Starting population exceeded valid map spawns.");
                Debug.Log("Entities attemped to spawn : " + startingPredatorPopulation);
                Debug.Log("Entities successfully spawned : " + i);
                break;
            }
            //Spawn entity
            int index = Random.Range(0, spawnZones.Count -1 );
            Vector3 spawnPos = spawnZones[index];
            spawnPos += Vector3.up * 0.75f;
            GameObject newEntity = Instantiate(predatorPrefab, spawnPos, Quaternion.identity);

            //Set gender
            int gender = Random.Range(0,2);
            newEntity.GetComponent<PredatorAgent>().Initialise(gender, GeneticsSystem.GetStartingPreyGenome());

            //Misc
            newEntity.transform.parent = this.transform;
            entities.Add(newEntity);
            spawnZones.Remove(spawnZones[index]);
            currentPopulation++;
        }
    }
    public void AddEntity(GameObject entity)
    {
        entities.Add(entity);
    }
    public void RemoveEntity(GameObject entity)
    {
        entities.Remove(entity);
    }
    public void CullAllEntities()
    {
        foreach(GameObject entity in entities)
        {
            entity.layer = 0;
        }
    }
    public void CullEntityList(List<GameObject> entitiesToCull)
    {
        CullAllEntities();
        foreach(GameObject entity in entitiesToCull)
        {
            entity.layer = 8;
        }
    }
}
