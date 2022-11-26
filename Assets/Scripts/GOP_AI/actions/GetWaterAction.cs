using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWaterAction : Action
{
    private Partition nearestWaterSource;
    private List<Partition> nearbyWaterSources = new List<Partition>();
    #region Common action methods
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "GetWater";
        agent = _agent;
        //Get all partitions in radius
        nearbyWaterSources = new List<Partition>();
        List<Partition> nearbyPartitions = agent.sensorySystem.GetVisionCone();
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
        agent.SetPerformingAction(true);
        nearestWaterSource = nearbyWaterSources[0];
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
        agent.StartCoroutine(i_waitUntilReachedWater());
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
    private void WalkToWater()
    {
        StartCoroutine(i_WalkToWater());
    }
    private IEnumerator i_WalkToWater()
    {
        float distance = Vector3.Distance(transform.position, nearestWaterSource.worldPosition);
        Rigidbody rb = agent.gameObject.GetComponent<Rigidbody>();
        Vector3 targetPos = nearestWaterSource.worldPosition;
        float time = 0;
        Vector3 lookPos = nearestWaterSource.worldPosition;
        lookPos.y = agent.transform.position.y;
        agent.transform.LookAt(lookPos);
        while(distance > 0.75f && Time.deltaTime < 2f)
        {   
            time+= Time.deltaTime;
            distance = Vector3.Distance(targetPos, transform.position);
            Vector3 velocity = targetPos - transform.position;
            velocity.Normalize();
            agent.setVelocity(velocity);
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector3.zero;
        DrinkWater();
    }
    private void DrinkWater()
    {
        StartCoroutine(i_DrinkWater());
    }
    private IEnumerator i_DrinkWater()
    {
        agent.SetDrinking(true);
        Vector3 targetPos = nearestWaterSource.worldPosition;
        Vector3 lookPos = nearestWaterSource.worldPosition;
        lookPos.y = agent.transform.position.y;
        agent.transform.LookAt(lookPos);
        agent.setVelocity(Vector3.zero);
        yield return new WaitForSeconds(0.5f);
        agent.SetPerformingAction(false);
        agent.SetDrinking(false);
        yield return null;
    }
    #endregion
}
