using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespondToCall : Action
{
    private enum ActionState { inactive,moving,findMate,walkToMate,mating, exit}
    private ActionState currentState = ActionState.inactive;
    private Vector3 targetLocation;
    private PreyAgent closestValidFemale;
    private float degrees = 0;
    public override bool isActionPossible(PreyAgent _agent)
    {
        actionName = "movingToMate";
        agent = _agent;
        //Agent is locked out of this action if too hungry, tired, thirsty
        if(agent.GetHunger() > 0.5 || agent.GetThirst() > 0.5 || agent.GetTiredness() > 0.5) { return false; }

        //Return false if agent doesn't have enough urge
        if(agent.GetReproductiveUrge() < 0.5){return false;}

        //Checking if the agent can hear any valid noises
        int gender = agent.GetGender();
        Vector2Int currPartition = agent.GetCurrentPartition();
        Partition partition = PartitionSystem.instance.partitions[currPartition.x, currPartition.y];
        foreach (noise currentNoise in partition.noises)
        {
            if (currentNoise.type == noise.noiseType.femalePreyMatingCall && gender == 0)
            {
                return true;
            }
            else if(currentNoise.type == noise.noiseType.malePreyMatingCall && gender == 1)
            {
                return true;
            }
        }
        return false;
    }
    public override float ActionScore()
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 95;
    }
    public override void PerformAction()
    {
        
        agent.SetPerformingAction(true);
        int gender = agent.GetGender();

        //Get the target noise
        noise.noiseType targetNoise;
        if(gender == 0)
        {
            targetNoise = noise.noiseType.femalePreyMatingCall;
        }
        else
        {
            targetNoise = noise.noiseType.malePreyMatingCall;
        }

        //Get nearest valid noise
        Partition localPartition = PartitionSystem.instance.partitions[agent.GetCurrentPartition().x, agent.GetCurrentPartition().y];
        float distance = 0, closest = float.MaxValue;
        foreach (noise currentNoise in localPartition.noises)
        {
            if(currentNoise.type != targetNoise){continue;}
            distance = Vector3.Distance(transform.position, currentNoise.sourceLocation);
            if (distance < closest)
            {
                closest = distance;
                targetLocation = currentNoise.sourceLocation;
            }
        }
        if(targetLocation == null) { currentState = ActionState.exit;}
        else 
        {
            targetLocation.y = transform.position.y; 
            currentState = ActionState.moving; 
        }

    }
    public override void UpdateAction()
    {
        agent.SetVelocity(Vector3.zero);
        switch (currentState)
        {
            case ActionState.inactive:
                break;
            case ActionState.moving:
                MoveToLocation();
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

    private void MoveToLocation()
    {
        //return if close to destination
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
                if(agent.GetComponent<Agent>().GetAgentType() == Agent.AgentType.PREDATOR){continue;}
                distance = Vector3.Distance(transform.position, agent.transform.position);
                if(distance < closest)
                {
                    closest = distance;
                    closestValidFemale = agent.gameObject.GetComponent<PreyAgent>();
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
            return;
        }
        else
        {
            currentState = ActionState.exit;
            return;
        }
    }
    private void WalkToMate()
    {
        if(closestValidFemale == null)
        {
            currentState = ActionState.exit;
            return;
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
            return;
        }
        closestValidFemale.TryMate(agent.GetGenome());
        currentState = ActionState.exit;
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
