using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private PlantSpawner spawner;
    private Vector2Int partition;
    private bool isBeingEaten = false;
    private int key;
    void Start()
    {
        key = Random.Range(0, int.MaxValue);
        spawner = PlantSpawner.instance;
    }
    public void setPartitionCoords(Vector2Int _partitionCoords)
    {
        partition = _partitionCoords;
    }
    public bool isEdible()
    {
        return !isBeingEaten;
    }
    public void startEating()
    {
        isBeingEaten = true;
    }
    public void stopEating()
    {
        isBeingEaten = false;
    }
    public Vector2Int GetCurrentPartition()
    {
        return partition;
    }
    public void Consume()
    {
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, partition, PartitionSystem.ObjectType.food);
        StartCoroutine(i_Respawn());
    }
    private IEnumerator i_Respawn()
    {
        Vector3 newPos = PartitionSystem.instance.GetValidSpawnZone();
        transform.position = newPos;
        float x,z;
        x = Random.Range(-.5f,.5f);
        z = Random.Range(-.5f,.5f);
        transform.position += new Vector3(x,0.6f,z);
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.food);
        partition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
        isBeingEaten = false;
        yield break;
    }
}
