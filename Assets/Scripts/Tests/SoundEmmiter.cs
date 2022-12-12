using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmmiter : MonoBehaviour
{
    private Vector2Int currentPartition;
    public void Start()
    {
        StartCoroutine(soundEmmiter());
    }
    void Update()
    {
        if(currentPartition == null)
        {
            PartitionSystem.instance.WorldToPartitionCoords(transform.position);
        }
        //Update position
        if(currentPartition != PartitionSystem.instance.WorldToPartitionCoords(transform.position))
        {
            //Unsubscribe from old partition
            currentPartition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
        }
    }
    public IEnumerator soundEmmiter()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            PartitionSystem.instance.EmitSound(2, transform.position, noise.noiseType.deathScream);
        }
    }
}
