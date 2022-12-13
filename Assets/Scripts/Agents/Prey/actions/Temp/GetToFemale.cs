using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetToFemale : Action
{
    private enum ActionState { inactive,moving, findFemale ,mating, exit}
    private ActionState currentState = ActionState.inactive;
    private Vector3 targetLocation;
    public override bool isActionPossible(PreyAgent _agent)
    {
        Vector2Int currPartition = agent.GetCurrentPartition();
        Partition partition = PartitionSystem.instance.partitions[currPartition.x, currPartition.y];
        foreach (noise currentNoise in partition.noises)
        {
            if (currentNoise.type == noise.noiseType.matingCall)
            {
                return true;
            }
        }
        return false;
    }
    public override float ActionScore()
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 85;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);

        //Get nearest valid female
        Partition localPartition = PartitionSystem.instance.partitions[agent.GetCurrentPartition().x, agent.GetCurrentPartition().y];
        float distance = 0, closest = float.MaxValue;
        foreach (noise currentNoise in localPartition.noises)
        {
            distance = Vector3.Distance(transform.position, currentNoise.sourceLocation);
            if (distance < closest)
            {
                closest = distance;
                targetLocation = currentNoise.sourceLocation;
            }
        }
        if(targetLocation == null) { currentState = ActionState.exit;}
        else { currentState = ActionState.moving; }
    }
    public override void UpdateAction()
    {
        switch (currentState)
        {
            case ActionState.inactive:
                break;
            case ActionState.moving:
                MoveToLocation();
                break;
            case ActionState.findFemale:
                FindFemale();
                break;
            case ActionState.mating:
                Mate();
                break;
            case ActionState.exit:
                ExitAction();
                break;
        }
    }

    private void MoveToLocation()
    {

    }
    private void FindFemale()
    {

    }
    private void Mate()
    {

    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
    }
    public override bool CanActionOverrideOthers()
    {
        return true;
    }
}
