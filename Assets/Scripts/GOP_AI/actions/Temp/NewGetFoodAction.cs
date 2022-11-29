using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGetFoodAction : Action
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
    private enum Stage{inactive, followPath, goToFood, eatFood, finished}
    private Stage currentStage = Stage.inactive;
    private List<Vector2Int> DijkstraPath = new List<Vector2Int>();
    #endregion
    public override bool isActionPossible(GOPAgent _agent)
    {
        agent = _agent;

        //Get all partitions that the agent is aware of
        List<Partition> nearbyPartitions = new List<Partition>();
        nearbyPartitions.AddRange(agent.sensorySystem.GetVisionCone());
        nearbyPartitions.AddRange(agent.sensorySystem.GetSmell());

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
        return (agent.hunger * agent.hunger *100) - distance;
    }
    public override void PerformAction()
    {
        currentStage = Stage.inactive;
        //Check if the agent needs to pathfind to the food or not
        if(agent.getCurrPartition() == nearestFoodPartition.coords)
        {
            //Move directly towards the food
            currentStage = Stage.goToFood;
        }
        else
        {
            //Get Path
            //Move along path
            currentStage = Stage.followPath;
        }
    }
    public override void UpdateAction()
    {
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
    #region actionStates
    private void FollowPath()
    {

    }
    private void GoToFood()
    {

    }
    private void EatFood()
    {

    }
    #endregion
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        currentStage = Stage.inactive;
    }
}
