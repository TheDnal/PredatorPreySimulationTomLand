using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyAgent : GOPAgent
{
    //GOPAgent that is a herbavore and avoids predators

    private PartitionSystem pSystem;
    private List<Partition> adjacentPartitions = new List<Partition>();
    private bool initialised = false;
    void Start()
    {
        Initialise();
    }

    public override void Initialise()
    {
        base.Initialise();
        pSystem = PartitionSystem.instance;
        currPartition = pSystem.WorldToPartitionCoords(transform.position);
        pSystem.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.agent);

        WanderAction wander = this.gameObject.AddComponent<WanderAction>();
        actions.Add(wander);

        GetFoodAction getFoodAction = this.gameObject.AddComponent<GetFoodAction>();
        actions.Add(getFoodAction);

        GetWaterAction getWaterAction = this.gameObject.AddComponent<GetWaterAction>();
        actions.Add(getWaterAction);

        ReproduceAction reproduceAction = this.gameObject.AddComponent<ReproduceAction>();
        actions.Add(reproduceAction);
        initialised = true;
        pathfindToDestination(transform.position + Vector3.forward * 2);
    }
    void Update()
    {
        if(!initialised)
        {
            return;
        }
        Vector2Int oldPartitionPos = currPartition;
        currPartition = pSystem.WorldToPartitionCoords(transform.position);
        if(oldPartitionPos != currPartition)
        {
            pSystem.RemoveGameObjectFromPartition(this.gameObject, oldPartitionPos, PartitionSystem.ObjectType.agent);
            pSystem.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.agent);
            adjacentPartitions = pSystem.GetPartitionsInRadius(transform.position, 1);
        }
        //transform.LookAt(transform.position + velocity);
        rb.velocity = velocity;
        if(performingAction)
        {
            return;
        }
        Action bestAction = CalculateBestAction();
        Debug.Log("Current action : " + bestAction.actionName);
        bestAction.PerformAction();
    }

    void OnDrawGizmos()
    {
        //debug to show that the adjacent partitions are correctly calculated
        
        // if(adjacentPartitions != null)
        // {
        //     Gizmos.color = Color.red;
        //     for(int i = 0; i < adjacentPartitions.Count; i++)
        //     {
        //         if(adjacentPartitions[i] != null)
        //         {
        //             Vector2Int displacement = adjacentPartitions[i].coords - currPartition;
        //             Vector3Int pos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 1, Mathf.RoundToInt(transform.position.z));
        //             pos.x += displacement.x;
        //             pos.z += displacement.y;
        //             Gizmos.DrawWireCube(pos, Vector3.one);
        //         }
        //     }
        // }
        if(currPos != null && destinationPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(currPos, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(destinationPos, Vector3.one);
        }
    }
}

