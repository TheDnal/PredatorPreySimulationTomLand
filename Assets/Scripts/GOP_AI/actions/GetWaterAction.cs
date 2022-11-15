using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWaterAction : Action
{
    public Vector3 nearestWaterSource = Vector3.up;
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "GetWater";
        agent = _agent;
        Vector2Int pos = agent.getCurrPartition();
        List<Partition> adjacentPartitions = PartitionSystem.instance.GetPartitionsInRadius(agent.transform.position, 5);
        float closest = 9999;
        float distance;
        Vector2Int closestPartition = Vector2Int.down * 100;
        foreach(Partition p in adjacentPartitions)
        {
            if(p.coords == pos){continue;}
            if(!p.HasWater()){continue;}
            distance = Vector2Int.Distance(p.coords, pos);
            if(distance < closest)
            {
                closest = distance;
                closestPartition = p.coords;
            }
        }
        if(closestPartition ==  Vector2Int.down * 100)
        {
            return false;
        }
        else
        {
            nearestWaterSource = PartitionSystem.instance.PartitionToWorldCoords(closestPartition); 
            return true;
        }
    }
    public override float ActionScore()
    {
        float score = (agent.GetThirst() *115) - Vector3.Distance(agent.transform.position, nearestWaterSource);
        return score;
    }
    public override void PerformAction()
    {
        actionRunning = true;
        StartCoroutine(moveToWater());
    }
    private IEnumerator moveToWater()
    {
        agent.SetPerformingAction(true);
        float distance = Vector3.Distance(agent.transform.position, nearestWaterSource);
        Vector3 velocity;
        float speed = agent.GetSpeedModifier();
        Rigidbody rb = agent.gameObject.GetComponent<Rigidbody>();
        while(distance > 1.4f)
        {
            velocity = nearestWaterSource - transform.position;
            velocity.Normalize();
            velocity *= speed;
            rb.velocity = velocity;
            distance = Vector3.Distance(agent.transform.position, nearestWaterSource);
            yield return new WaitForEndOfFrame();
        }
        agent.SetPerformingAction(true);
        agent.SetDrinking(true);
        yield return new WaitForSeconds(1f);
        agent.SetDrinking(false);
        agent.SetPerformingAction(false);
        actionRunning = false;
    }

    void OnDrawGizmos()
    {
        if(nearestWaterSource != null && actionRunning && agent.showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(agent.transform.position, nearestWaterSource);
            Gizmos.DrawWireCube(nearestWaterSource, Vector3.one);
        }
    }
}
