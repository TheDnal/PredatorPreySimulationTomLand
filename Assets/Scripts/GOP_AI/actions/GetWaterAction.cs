using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWaterAction : Action
{
    public Vector3 nearestWaterSource = Vector3.up;
    private List<Partition> nearbyWaterSources = new List<Partition>();
    #region Common action methods
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "GetWater";
        agent = _agent;
        //Get all partitions in radius
        nearbyWaterSources = new List<Partition>();
        List<Partition> nearbyPartitions = PartitionSystem.instance.GetPartitionsInRadius(agent.transform.position, 3);
        //Check if any contain water
        foreach(Partition p in nearbyPartitions)
        {
            if(p.hasDrinkbleWater())
            {
                nearbyWaterSources.Add(p);
            }
        }
        //If no water sources nearby, return false
        if(nearbyWaterSources.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override float ActionScore()
    {
        float score = (agent.GetThirst() *100);
        return score;
    }
    public override void PerformAction()
    {
        Partition nearestWaterSource = nearbyWaterSources[0];
        float distance = 0, closest = float.MaxValue;
        foreach(Partition p in nearbyWaterSources)
        {
            distance = Vector3.Distance(agent.transform.position, p.worldPosition);
            if(distance < closest)
            {
                closest = distance;
                nearestWaterSource = p;
            }
        }
        if(PartitionSystem.instance.WorldToPartitionCoords(nearestWaterSource.worldPosition) == agent.getCurrPartition())
        {
            DrinkWater();
            return;
        }
        if(!agent.isTargetReachable(nearestWaterSource.worldPosition))
        {
            agent.SetPerformingAction(false);
            return;
        }
        actionRunning = true;
        agent.PathToTarget(nearestWaterSource.worldPosition);

    }
    #endregion
    #region Reaching and drinking water
    private IEnumerator i_waitUntilReachedWater()
    {
        while(!agent.arrivedAtDestination)
        {
            yield return new WaitForEndOfFrame();
        }
        agent.arrivedAtDestination = false;
        DrinkWater();
    }
    private void DrinkWater()
    {
        StartCoroutine(i_DrinkWater());
    }
    private IEnumerator i_DrinkWater()
    {
        agent.SetPerformingAction(true);
        agent.SetDrinking(true);
        yield return new WaitForSeconds(0.5f);
        agent.SetPerformingAction(false);
        agent.SetDrinking(false);
        yield return null;
    }
    #endregion
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
