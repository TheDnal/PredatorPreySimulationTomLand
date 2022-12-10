using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    private Vector2Int currentPartition;
    void Start()
    {
        test();
    }
    void Update()
    {
        if(currentPartition == null)
        {
            PartitionSystem.instance.WorldToPartitionCoords(transform.position);
            PartitionSystem.instance.partitions[currentPartition.x,currentPartition.y].ChangeDangerSourceCount(1);
        }
        //Update position
        if(currentPartition != PartitionSystem.instance.WorldToPartitionCoords(transform.position))
        {
            //Unsubscribe from old partition
            PartitionSystem.instance.partitions[currentPartition.x,currentPartition.y].ChangeDangerSourceCount(-1);
            currentPartition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
            PartitionSystem.instance.partitions[currentPartition.x,currentPartition.y].ChangeDangerSourceCount(1);
        }
    }
    ///<summary> Testing Method </summary>
    public void test()
    {

    }
}

