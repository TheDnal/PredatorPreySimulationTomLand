using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFoodAction : Action
{
    private GameObject nearestFoodObject;
    private float startTime;
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionRunning = false;
        actionName = "GetFood";
        agent = _agent;
        //Get partitions in radius
        List<Partition> partitions = agent.sensorySystem.GetVisionCone();

        //Get nearest plant object that isn't being eaten
        float closest = 9999, distance;
        nearestFoodObject = null;
        foreach(Partition p in partitions)
        {
            foreach(GameObject food in p.food)
            {
                if(food.TryGetComponent(out Plant currPlant))
                {
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
        }
        //Return true if a valid food object is spotted, else return false
        return nearestFoodObject == null ? false : true;
    }
    public override float ActionScore()
    {
        //Return hunger - distance to nearest food object
        float score = (agent.GetHunger() * 125);
        return score;
    }
    public override void PerformAction()
    {
        startTime = Time.time;
        agent.SetPerformingAction(true);
        if(PartitionSystem.instance.WorldToPartitionCoords(nearestFoodObject.transform.position) == agent.getCurrPartition())
        {
            WalkToFood();
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
        agent.arrivedAtDestination = false;
        WalkToFood();
    }
    private void WalkToFood()
    {
        StartCoroutine(i_WalkToFood());
    }
    private IEnumerator i_WalkToFood()
    {
        float distance = Vector3.Distance(transform.position, nearestFoodObject.transform.position);
        Rigidbody rb = agent.gameObject.GetComponent<Rigidbody>();
        Vector3 targetPos = nearestFoodObject.transform.position;
        float time = 0;
        Vector3 lookPos = nearestFoodObject.transform.position;
        lookPos.y = agent.transform.position.y;
        agent.transform.LookAt(lookPos);
        while(distance > 0.20f && time <= 2f)
        {   
            time+= Time.deltaTime;
            distance = Vector3.Distance(targetPos, transform.position);
            Vector3 velocity = targetPos - transform.position;
            velocity.Normalize();
            rb.velocity = velocity;
            yield return new WaitForEndOfFrame();
        }
        if(time > 2f)
        {
            agent.SetPerformingAction(false);
            Debug.Log("i_WalkToFood aborted");
            yield return null;
        }
        rb.velocity = Vector3.zero;
        ConsumeFood();
    }
    private void ConsumeFood()
    {
        StartCoroutine(i_ConsumeFood());
    }
    private IEnumerator i_ConsumeFood()
    {
        if(nearestFoodObject.TryGetComponent(out Plant plant))
        {
            if (plant.GetCurrentPartition() != agent.getCurrPartition())
            {
                agent.SetPerformingAction(false);
                yield return null;
            }
            if(plant.isEdible())
            {
                agent.setVelocity(Vector3.zero);
                Vector3 lookPos = nearestFoodObject.transform.position;
                lookPos.y = agent.transform.position.y;
                agent.transform.LookAt(lookPos);
                plant.startEating();
                agent.SetEating(true);
                yield return new WaitForSeconds(1f);
                plant.Consume();
                agent.SetPerformingAction(false);
                agent.SetEating(false);
            }
            else
            {
                yield return null;
            }
        }

    }
    void OnDrawGizmos()
    {
        if(nearestFoodObject != null && actionRunning && agent.showGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(agent.transform.position, nearestFoodObject.transform.position);
            Gizmos.DrawWireCube(nearestFoodObject.transform.position, Vector3.one);
        }
    }
}
