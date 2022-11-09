using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFoodAction : Action
{
    private GameObject nearestFoodObject;
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionRunning = false;
        actionName = "GetFood";
        agent = _agent;
        //Get partitions in radius
        List<Partition> partitions = PartitionSystem.instance.GetPartitionsInRadius(transform.position, 2);

        //Get nearest plant object that isn't being eaten
        float closest = 9999, distance;
        Plant currPlant;
        nearestFoodObject = null;
        foreach(Partition p in partitions)
        {
            foreach(GameObject food in p.food)
            {
                if(food == null)
                {
                    continue;
                }
                currPlant = food.GetComponent<Plant>();
                if(currPlant.isEdible())
                {
                    distance = Vector3.Distance(agent.transform.position, food.transform.position);
                    if(distance < closest)
                    {
                        closest = distance;
                        nearestFoodObject = food;
                    }
                }
            }
        }
        //Return true if a valid food object is spotted, else return false
        return nearestFoodObject == null ? false : true;
    }
    public override float ActionScore()
    {
        //Return hunger - distance to nearest food object
        float score = (agent.GetHunger() * 125) - Vector3.Distance(transform.position, nearestFoodObject.transform.position) * 5;
        return score;
    }
    public override void PerformAction()
    {
        if(PartitionSystem.instance.WorldToPartitionCoords(nearestFoodObject.transform.position) == agent.getCurrPartition())
        {
            ConsumeFood();
            return;
        }
        if(!agent.isTargetReachable(nearestFoodObject.transform.position))
        {
            agent.SetPerformingAction(false);
            return;
        }
        actionRunning = true;
        agent.PathToTarget(nearestFoodObject.transform.position);
        StartCoroutine(i_waitUntilReachedFood());
    }
    private IEnumerator i_waitUntilReachedFood()
    {
        while(!agent.arrivedAtDestination)
        {
            yield return new WaitForEndOfFrame();
        }
        ConsumeFood();
    }
    private void ConsumeFood()
    {
        StartCoroutine(i_ConsumeFood());
    }
    private IEnumerator i_ConsumeFood()
    {
        agent.SetPerformingAction(true);
        agent.SetEating(true);
        yield return new WaitForSeconds(.5f);
        Plant plant = nearestFoodObject.GetComponent<Plant>();
        plant.Consume();
        agent.SetEating(false);
        agent.SetPerformingAction(false);
    }
    void OnDrawGizmos()
    {
        if(nearestFoodObject != null && actionRunning)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(agent.transform.position, nearestFoodObject.transform.position);
            Gizmos.DrawWireCube(nearestFoodObject.transform.position, Vector3.one);
        }
    }
}
