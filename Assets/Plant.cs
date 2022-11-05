using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private PlantSpawner spawner;
    private Vector2Int partition;
    private bool isBeingEaten = false;
    void Start()
    {
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
    public void Consume()
    {
        spawner.DecrementPlantCount();
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, partition, PartitionSystem.ObjectType.food);
        Destroy(this.gameObject);
    }
}
