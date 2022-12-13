using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWaterAction : Action
{
    /*
        This Action class will attempt to find, get to, and drink water.
        This is broken down into the following steps:
        1) Look for water with the agents vision
        2) find the closest water source
        3) Get the fastest path (Djikstra) to the water source
        4) Move to the water source
        5) drink from the water source
        6) Exit the action, and return control to the agent

        If, at any stage the action is no longer possible, for example if the agent
        spots a predator, the action is aborted and control is returned to the agent
    */
    #region fields
    private Partition nearestWaterSource = null;
    private enum ACTION_STAGE{inactive, followPath, GetToWaterSource,DrinkWater, finished}
    private ACTION_STAGE currentStage = ACTION_STAGE.inactive;
    private List<Vector2Int> DijkstraPath = new List<Vector2Int>();
    private Vector2Int startPosition;
    private Vector2Int targetWaypoint;
    private Vector3 targetPos;
    private float timer = 0;
    #endregion
    #region Inherrited methods
    public override bool isActionPossible(PreyAgent _agent)
    {
        actionName = "GetWater";
        nearestWaterSource = null;
        agent = _agent;
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        float distance = 0;
        float closest = float.MaxValue;
        foreach(Partition partition in visiblePartitions)
        {
            if(partition.hasDrinkbleWater())
            {  
                distance = Vector3.Distance(transform.position, partition.worldPosition);
                if(distance < closest)
                {
                    closest = distance;
                    nearestWaterSource = partition;
                }
            }
        } 
        if(nearestWaterSource != null)
        {
            return true;
        }
        return false;
    }
    public override float ActionScore()
    {
        float distance = Vector3.Distance(transform.position, nearestWaterSource.worldPosition);
        return (agent.GetThirst() * agent.GetThirst() * 100) - distance;
    }
    public override void PerformAction()
    {
        timer = 0;
        agent.SetVelocity(Vector3.zero);
        agent.SetPerformingAction(true);
        targetWaypoint = new Vector2Int(-1,-1);
        startPosition = agent.GetCurrentPartition();
        //Check if current partition has water
        Vector2Int pos = agent.GetCurrentPartition();
        Partition currPartition = PartitionSystem.instance.partitions[pos.x,pos.y];
        if(currPartition.hasDrinkbleWater())
        {
            currentStage = ACTION_STAGE.DrinkWater;
            return;
        }
        
        //Check if pathfinding is necessary
        if(agent.GetCurrentPartition() == nearestWaterSource.coords)
        {
            //Move directly to water front
            currentStage = ACTION_STAGE.GetToWaterSource;
        }
        else
        {
            //Get path
            DijkstraPath = agent.GetPathfindingSystem().GetPathToPartition(nearestWaterSource.coords, 3);
            //Move along path
            currentStage = ACTION_STAGE.followPath;
        }
    }
    public override void UpdateAction()
    {
        switch(currentStage)
        {
            case ACTION_STAGE.inactive:
                return;
            case ACTION_STAGE.followPath:
                FollowPath();
                return;
            case ACTION_STAGE.GetToWaterSource:
                GetToWaterSource();
                return;
            case ACTION_STAGE.DrinkWater:
                DrinkWater();
                return;
            case ACTION_STAGE.finished:
                ExitAction();
                return;
        }
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        agent.SetDrinking(false);
        currentStage = ACTION_STAGE.inactive;
    }
    public override bool CanActionOverrideOthers()
    {
        return true;
    }
    #endregion
    #region ActionStates
    private void FollowPath()
    {
        //If starting the path, set first waypoint
        if(targetWaypoint == new Vector2Int(-1,-1))
        {
            if(DijkstraPath == null)
            {
                currentStage = ACTION_STAGE.finished;
                return;
            }
            targetWaypoint = DijkstraPath[0];
        }

        //Check if agent has reached current waypoint
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

        //If they have, move onto the next waypoint
        else
        {
            //Get index of next position
            int index = DijkstraPath.IndexOf(targetWaypoint) + 1;

            //iterate to next waypoint
            if(index < DijkstraPath.Count)
            {
                targetWaypoint = DijkstraPath[index];
            }
            //Reached final waypoint
            else
            {
                currentStage = ACTION_STAGE.GetToWaterSource;
            }
        }
    }
    private void GetToWaterSource()
    {
        //Walk to water front
        //Pythagorean theorem, closest distance to water front is 0.707, so wait until distance = 0.85

        //Get distance
        Vector3 targetPosition = nearestWaterSource.worldPosition;
        targetPosition.y = transform.position.y;
        targetPos = targetPosition;
        float distance = Vector3.Distance(transform.position , targetPosition);
        //If too far away, keep approaching water front
        if(distance > 2f)
        {
            Vector3 velocity = targetPosition - transform.position;
            velocity.y = 0;
            velocity.Normalize();
            agent.SetVelocity(velocity);
        }
        //Otherwise, drink
        else
        {
            agent.SetVelocity(Vector3.zero);
            currentStage = ACTION_STAGE.DrinkWater;
        }
    }   
    private void DrinkWater()
    {
        agent.SetDrinking(true);
        timer+= Time.deltaTime;
        if(timer >= 1f)
        {
            agent.SetDrinking(false);
            currentStage = ACTION_STAGE.finished;
        }
    }
    #endregion
    #region Misc
    void OnDrawGizmos()
    {
        if(currentStage != ACTION_STAGE.inactive && agent == Agent.selectedAgent)
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
