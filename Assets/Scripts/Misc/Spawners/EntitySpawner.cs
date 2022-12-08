using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    //AI Entity spawner Class. Gets a list of valid spawn points from the map generator and spawns AI on those spots
    public MapGenerator generator;
    public int startingPopulation, currentPopulation;
    public GameObject entityPrefab;
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
            //Spawn entity
            int index = Random.Range(0, spawnZones.Count -1 );
            Vector3 spawnPos = spawnZones[index];
            spawnPos += Vector3.up * 0.75f;
            GameObject newEntity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);

            //Set gender
            int gender = Random.Range(0,2);
            Material genderMat;
            if(gender == 0)
            {
                genderMat = MaleMat;
            }
            else
            {
                genderMat = FemaleMat;
            }
            newEntity.GetComponent<MeshRenderer>().material = genderMat;
            newEntity.GetComponent<PreyAgent>().Initialise(gender, GeneticsSystem.GetStartingPreyGenome());

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
