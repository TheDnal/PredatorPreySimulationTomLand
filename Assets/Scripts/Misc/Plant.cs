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
        key = Random.Range(0, int.MaxValue);
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, partition, PartitionSystem.ObjectType.food);
        StartCoroutine(i_Respawn());
    }
    private IEnumerator i_Respawn()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newPos = PlantSpawner.instance.getValidSpawn();
        transform.position = newPos;
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.food);
        partition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
    }
}
