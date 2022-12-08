using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFoodAction : Action
{
    /*
        This Action class will attempt to find, get to, and consume food.
        This is broken down into the following steps:
        1) Check for food in the agents vision and smell senses
        2) Find the closest food object
        3) Get the fastest path (Djikstra) to the food object
        4) Move to the food object
        5) Eat the food object
        6) Exit the action, and return control to the agent

        If, at any stage the action is no longer possible, for example if the target 
        food object is eaten by another agent, the action is aborted and control
        is returned to the agent.
        
    */
    #region Fields
    private Partition nearestFoodPartition = null;
    private GameObject nearestFoodObject = null;
    private enum Stage{inactive, followPath, goToFood, eatFood, finished}
    private Stage currentStage = Stage.inactive;
    private List<Vector2Int> DijkstraPath = new List<Vector2Int>();
    private Vector2Int startPosition;
    #endregion
    #region Inherrited Methods
    public override bool isActionPossible(NewPreyAgent _agent)
    {
        agent = _agent;
        actionName = "GetFood";
        //Get all partitions that the agent is aware of
        List<Partition> nearbyPartitions = new List<Partition>();
        nearbyPartitions.AddRange(agent.GetSensorySystem().GetVisionCone());
        nearbyPartitions.AddRange(agent.GetSensorySystem().GetSmell());

        //Iterate over all of them to look for food, and get the closest one if it does
        nearestFoodPartition = null;
        float closest = float.MaxValue;
        float distance = 0;
        foreach(Partition partition in nearbyPartitions)
        {
            if(partition.HasFood())
            {
                distance = Vector3.Distance(transform.position, partition.worldPosition);
                if(distance < closest)
                {
                    closest = distance;
                    nearestFoodPartition = partition;
                }
            }
        }

        //Return true if there is a nearest food, false if not
        return nearestFoodPartition != null ? true : false;
    }
    public override float ActionScore()
    {
        //Food insistence = agentHunger^2 - distance
        //agentHunger is squared to make high insistence values much stronger
        float distance = Vector3.Distance(transform.position, nearestFoodPartition.worldPosition);
        return (agent.GetHunger() * agent.GetHunger() *100) - distance;
    }
    public override void PerformAction()
    {
        timer = 0;
        startPosition = agent.GetCurrentPartition();
        agent.SetVelocity(Vector3.zero);
        agent.SetPerformingAction(true);
        nearestFoodObject = null;
        currentStage = Stage.inactive;
        targetWaypoint = new Vector2Int(-1,-1);
        //Check if the agent needs to pathfind to the food or not
        if(agent.GetCurrentPartition() == nearestFoodPartition.coords)
        {
            //Move directly towards the food
            currentStage = Stage.goToFood;
        }
        else
        {
            //Get Path
            DijkstraPath = agent.GetPathfindingSystem().GetPathToPartition(nearestFoodPartition.coords,3);
            //Move along path
            currentStage = Stage.followPath;
        }
    }
    public override void UpdateAction()
    {
        if(!IsActionStillValid())
        {
            ExitAction();
        }
        switch(currentStage)
        {
            case Stage.inactive:
                return;
            case Stage.followPath:
                FollowPath();
                break;
            case Stage.goToFood:
                GoToFood();
                break;
            case Stage.eatFood:
                EatFood();
                break;
            case Stage.finished:
                ExitAction();
                break;
        }
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        currentStage = Stage.inactive;
    }
    #endregion
    #region actionStates
    private Vector2Int targetWaypoint;
    private void FollowPath()
    {

        //If starting the path, set the first waypoint
        if(targetWaypoint == new Vector2Int(-1,-1))
        {
            targetWaypoint = DijkstraPath[0];
        }

        //Check if agent has reached the current waypoint
        Vector3 targetPosition = PartitionSystem.instance.PartitionToWorldCoords(targetWaypoint + startPosition);
        targetPosition.y = transform.position.y;
        float distance = Vector3.Distance(transform.position, targetPosition);

        //if not, keep going
        if(distance > 0.25f)
        {
            Vector3 velocity = targetPosition - transform.position;
            velocity.y = 0;
            velocity.Normalize();
            agent.SetVelocity(velocity);
        }

        //if they have, move onto the next waypoint
        else
        {
            //Get index of next position
            int index = DijkstraPath.IndexOf(targetWaypoint) + 1;

            //Iterate to next waypoint
            if(index < DijkstraPath.Count - 1)
            {
                targetWaypoint = DijkstraPath[index];
            }
            //Reached final waypoint, move to next state
            else
            {   
                currentStage = Stage.goToFood;
            }
        }
    }
    private void GoToFood()
    { 
        //pick a random food object in the partition
        if(nearestFoodObject == null)
        {
            int index = Random.Range(0, nearestFoodPartition.foodCount - 1);
            nearestFoodObject = nearestFoodPartition.food[index];
        }

        //Walk to food
        float distance = Vector3.Distance(transform.position, nearestFoodObject.transform.position);
        if(distance > 0.2f)
        {
            Vector3 velocity = nearestFoodObject.transform.position - transform.position;
            velocity.y = 0;
            velocity.Normalize();
            agent.SetVelocity(velocity);
        }
        //Continue until reaching it
        else
        {
            agent.SetVelocity(Vector3.zero);
            currentStage = Stage.eatFood;
        }
    }
    private float timer = 0;
    private void EatFood()
    {
        agent.SetEating(true);
        timer+= Time.deltaTime;
        if(nearestFoodObject.GetComponent<Plant>().isEdible())
        {
            nearestFoodObject.GetComponent<Plant>().startEating();
        }
        //Wait until time elapsed then finish state
        if(timer >= 1)
        {
            nearestFoodObject.GetComponent<Plant>().Consume();
            agent.SetEating(false);
            currentStage = Stage.finished;
        }
        
    }
    #endregion
    #region misc
    private bool IsActionStillValid()
    {
        if(nearestFoodObject != null)
        {
            if(!nearestFoodObject.GetComponent<Plant>().isEdible() && currentStage != Stage.eatFood)
            {
            //Someone else has beat the agent to the food
            return false;
            }
        }
        
        if(nearestFoodPartition.foodCount == 0)
        {
            //No food in partition
            return false;
        }
        return true;
    }
    void OnDrawGizmos()
    {
        if(currentStage != Stage.inactive)
        {
            Gizmos.color = Color.white;
            if(DijkstraPath != null)
            {
                foreach(Vector2Int pos in DijkstraPath)
                {
                    Vector3 worldPos = PartitionSystem.instance.PartitionToWorldCoords(pos + startPosition);
                    Gizmos.DrawWireCube(worldPos,Vector3.one);
                }
            }
        }
    }
    #endregion
}
