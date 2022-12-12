using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvFindWater : AdvancedAction
{
    /*
        Advanced drink action. This advanced action moves
        the GOAP agent towards a water source and drink
    */
    #region Variables
    private Partition nearestWaterSource = null;
    private enum ActionStage{INACTIVE,FOLLOWPATH,DRINK,EXIT}
    private ActionStage currentStage = ActionStage.INACTIVE;
    private List<Vector2Int> DijkstraPath = new List<Vector2Int>();
    private Vector2Int StartPosition, targetWaypoint;
    private Vector3 targetPos;
    private float timer = 0;
    #endregion
    #region Methods
    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "GetWater";
    }
    public override bool isActionPossible(PredatorDiscontentSnapshot snapshot)
    {
        nearestWaterSource = null;
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        float distance = 0, closest = float.MaxValue;
        foreach(Partition partition in visiblePartitions)
        {
            if(partition.hasDrinkbleWater())
            {
                distance = Vector3.Distance(partition.worldPosition, transform.position);
                if(distance< closest)
                {
                    closest = distance;
                    nearestWaterSource = partition;
                }
            }
        }
        if(nearestWaterSource != null){return true;}
        return false;
    }
    public override float ActionScore(PredatorDiscontentSnapshot snapshot)
    {
        float distance = Vector3.Distance(transform.position, nearestWaterSource.worldPosition);        
        return (snapshot.GetThirst() * snapshot.GetThirst() * 110) - distance;
    }
    public override float EstimatedDuration(PredatorDiscontentSnapshot snapshot)
    {
        return Vector3.Distance(transform.position, nearestWaterSource.worldPosition);
    }
    public override void PerformAction()
    {
        timer = 0;
        agent.SetVelocity(Vector3.zero);
        agent.SetPerformingAction(true);
        targetWaypoint = new Vector2Int(-1,-1);
        StartPosition = agent.GetCurrentPartition();
        Partition currPartition = PartitionSystem.instance.partitions[StartPosition.x,StartPosition.y];
        if(currPartition.hasDrinkbleWater())
        {
            currentStage = ActionStage.DRINK;
            return;
        }
        else
        {
            DijkstraPath = agent.GetPathfindingSystem().GetPathToPartition(nearestWaterSource.coords,5);
            currentStage = ActionStage.FOLLOWPATH;
        }
    }
    public override void UpdateAction()
    {
        switch(currentStage)
        {
            case ActionStage.INACTIVE:
                return;
            case ActionStage.FOLLOWPATH:
                FollowPath();
                return;
            case ActionStage.DRINK:
                Drink();
                return;
            case ActionStage.EXIT:
                ExitAction();
                return;
        }
    }
    private void FollowPath()
    {
        //Start
        if(targetWaypoint == new Vector2Int(-1,-1))
        {
            targetWaypoint = DijkstraPath[0];
        }
        //Check if reached waypoint
        Vector3 targetPos = PartitionSystem.instance.PartitionToWorldCoords(targetWaypoint + StartPosition);
        targetPos.y = transform.position.y;
        float distance = Vector3.Distance(transform.position, targetPos);
        //If not, keep moving towards waypoint
        if(distance > 0.25f)
        {
            Vector3 velocity = targetPos - transform.position;
            velocity.y = 0;
            velocity.Normalize();
            agent.SetVelocity(velocity);
            return;
        }
        //Else move onto next waypoint
        int index = DijkstraPath.IndexOf(targetWaypoint) + 1;
        //Have they reached the destination?
        if(index < DijkstraPath.Count)
        {
            targetWaypoint = DijkstraPath[index];
            return;
        }
        currentStage = ActionStage.DRINK;
    }
    private void Drink()
    {
        agent.SetDrinking(true);
        agent.SetVelocity(Vector3.zero);
        timer += Time.deltaTime;
        if(timer >= 1f)
        {
            agent.SetDrinking(false);
            currentStage = ActionStage.EXIT;
        }
    }
    public override void ExitAction()
    {
        agent.SetDrinking(false);
        agent.SetPerformingAction(false);
        currentStage = ActionStage.INACTIVE;
    }
    #endregion
}
