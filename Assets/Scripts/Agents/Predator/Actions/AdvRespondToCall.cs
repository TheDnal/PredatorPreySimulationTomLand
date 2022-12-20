using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvRespondToCall : AdvancedAction
{
    private enum ActionState {inactive, moving, findMate, walkToMate, mating, exit}
    private ActionState currentState = ActionState.inactive;
    private Vector3 targetLocation;
    private PredatorAgent closestValidFemale;
    private float degrees = 0;
    private float timer =0;
    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "movingToMate";
    }
    public override bool isActionPossible(PredatorDiscontents snapshot, bool isChainAction)
    {
        //Return false if agent is too hungry, tired etc.
        if(snapshot.GetHunger() > 0.75f || snapshot.GetThirst() > 0.75f || snapshot.GetTiredness() > 0.75f || snapshot.GetReproductiveUrge() < 0.75f){return false;}
        

        //Check if the agent can hear any calls to respond to
        int gender = agent.GetGender();
        Vector2Int currPartitionCoords = agent.GetCurrentPartition();
        Partition partition = PartitionSystem.instance.partitions[currPartitionCoords.x,currPartitionCoords.y];
        foreach(noise currentNoise in partition.noises)
        {
            if(currentNoise.type == noise.noiseType.femalePredatorMatingCall && gender == 0)
            {
                return true;
            }
            if(currentNoise.type == noise.noiseType.malePredatorMatingCall && gender == 1)
            {
                return true;
            }
        }
        return false;
    }
    public override float ActionScore(PredatorDiscontents snapshot, bool isChainAction)
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 160;
    }
    public override void PerformAction()
    {
        timer = 0;
        agent.SetPerformingAction(true);
        //Get target noise
        int gender = agent.GetGender();
        noise.noiseType targetNoise;
        if(gender == 0){targetNoise = noise.noiseType.femalePredatorMatingCall;}
        else{targetNoise = noise.noiseType.malePredatorMatingCall;}

        //Get nearest valid noise
        Partition localPartition = PartitionSystem.instance.partitions[agent.GetCurrentPartition().x,agent.GetCurrentPartition().y];
        float distance = 0, closest = float.MaxValue;
        foreach(noise currentNoise in localPartition.noises)
        {
            distance = Vector3.Distance(transform.position, currentNoise.sourceLocation);
            if(distance < closest)
            {
                targetLocation = currentNoise.sourceLocation;
                closest = distance;
            }
        }
        if(targetLocation == null){currentState = ActionState.exit;}
        else
        {
            targetLocation.y = transform.position.y;
            currentState = ActionState.moving;
        }
    }
    public override void UpdateAction()
    {
        timer += Time.deltaTime;
        if(timer > 5){currentState = ActionState.exit;}
        agent.SetVelocity(Vector3.zero);
        switch(currentState)
        {
            case ActionState.inactive:
                break;
            case ActionState.moving:
                MoveTolocation();
                break;
            case ActionState.findMate:
                FindMate();
                break;
            case ActionState.walkToMate:
                WalkToMate();
                break;
            case ActionState.mating:
                Mate();
                break;
            case ActionState.exit:
                ExitAction();
                break;
        }
    }
    private void MoveTolocation()
    {
        //Returns if close to destination
        if(Vector3.Distance(transform.position, targetLocation) < 0.25f)
        {
            currentState = ActionState.findMate;
            return;
        }
        Vector3 velocity = targetLocation - transform.position;
        velocity.Normalize();
        agent.SetVelocity(velocity);
    }
    private void FindMate()
    {
        //Check if can see female
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        float closest = float.MaxValue, distance = 0;
        foreach(Partition currentPartition in visiblePartitions)
        {
            if(currentPartition.agents.Count == 0){continue;}
            foreach(GameObject agent in currentPartition.agents)
            {
                if(agent.GetComponent<Agent>().GetAgentType() == Agent.AgentType.PREY){continue;}
                distance = Vector3.Distance(transform.position,agent.transform.position);
                if(distance < closest)
                {
                    closest = distance;
                    closestValidFemale = agent.gameObject.GetComponent<PredatorAgent>();
                }
            }
        }
        if(closestValidFemale == null)
        {
            currentState = ActionState.exit;
            return;
        }
        else if(closestValidFemale.TryBecomeMate(agent))
        {
            currentState = ActionState.walkToMate;
        }        
        else
        {
            currentState = ActionState.exit;    
        }
    }
    private void WalkToMate()
    {
        if(closestValidFemale == null)
        {
            currentState = ActionState.exit;
        }
        Vector3 velocity = closestValidFemale.transform.position - transform.position;
        if(Vector3.Distance(transform.position, closestValidFemale.transform.position) < 0.1)
        {
            currentState = ActionState.mating;
            return;
        }
        agent.SetVelocity(velocity);
    }
    private void Mate()
    {
        if(closestValidFemale == null)
        {
            currentState = ActionState.exit;
        }
        closestValidFemale.TryMate(agent.GetGenome());
        agent.ResetReproductiveUrge();
        currentState = ActionState.exit;
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        currentState = ActionState.inactive;
    }
    public override float EstimatedDuration(PredatorDiscontents snapshot)
    {
        return 5;
    }
}
