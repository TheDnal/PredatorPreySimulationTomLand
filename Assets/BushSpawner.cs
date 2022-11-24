using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushSpawner : MonoBehaviour
{
    public static BushSpawner instance;
    public int BushPopulation = 50;
    public GameObject bushPrefab;
    public float fruitGrowthTime = 5;
    [Range(0,3)]
    public int maxFruitPerBush = 3;
    void Awake()
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
    }
    public void Initialise()
    {
        Bush.SetStaticVariables(fruitGrowthTime, maxFruitPerBush);
        SpawnBushes();
    }
    private void SpawnBushes()
    {
        List<Vector3> spawns = MapGenerator.instance.GetValidSpawnZones();
        for(int i = 0; i < BushPopulation; i++)
        {
            int index = Random.Range(0,spawns.Count - 1);
            float x = Random.Range(-4,4);
            float z = Random.Range(-4,4);
            Vector3 pos = new Vector3(x/10,.5f,z/10);
            pos += spawns[index];
            spawns.Remove(spawns[index]);
            float rotation = Random.Range(0,3) * 90;
            Quaternion rot = Quaternion.Euler(0,rotation,0);
            GameObject newBush = Instantiate(bushPrefab, pos, rot);
            newBush.transform.parent = this.transform;
            newBush.name = "Bush #" + i;
            newBush.transform.localScale *= 0.5f;
            PartitionSystem.instance.AddGameObjectToPartition(newBush,PartitionSystem.ObjectType.food);
        }
    }
}
