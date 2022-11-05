using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFoodAction : Action
{
    private GameObject nearestFoodObject;
    public override bool isActionPossible(GOPAgent _agent)
    {
        agent = _agent;
        //Get partitions in radius
        Vector2Int currPartition = agent.getCurrPartition();
        List<Partition> partitions = PartitionSystem.instance.GetPartitionsInRadius(transform.position, 1);

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
        float score = (agent.GetHunger() * 100) - Vector3.Distance(transform.position, nearestFoodObject.transform.position) * 10;
        return score;
    }
    public override void PerformAction()
    {
        StartCoroutine(goToFood());
    }
    private IEnumerator goToFood()
    {
        agent.SetPerformingAction(true);
        Vector3 target = nearestFoodObject.transform.position;
        Plant targetFood = nearestFoodObject.GetComponent<Plant>();
        float speed = agent.GetSpeedModifier();
        Vector3 velocity;
        Rigidbody rb = agent.GetComponent<Rigidbody>();
        while(targetFood.isEdible())
        {
            if(Vector3.Distance(transform.position, target)< 0.4f)
            {
                break;
            }
            velocity = target - transform.position;
            velocity.Normalize();
            velocity *= speed;
            rb.velocity = velocity;
            yield return new WaitForEndOfFrame();
        }
        if(Vector3.Distance(transform.position, target) < 0.4)
        {
            targetFood.startEating();
            agent.SetEating(true);
            yield return new WaitForSeconds(0.3f);
            agent.SetEating(false);
            targetFood.Consume();
        }
        agent.SetPerformingAction(false);
        yield return null;
    }
}
