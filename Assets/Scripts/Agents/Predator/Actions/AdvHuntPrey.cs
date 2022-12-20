using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvHuntPrey : AdvancedAction
{
    /*
        Advanced Hunt action. This advanced action has the GOAP
        agent select a target prey, then will attempt to catch, kill
        and eat it. The strategy is as follows :
        -Will attempt to get very close to the prey, whilst moving slowly
        -If the prey spots the agent, or the agent gets close enough, the 
        agent will charge the prey.
        -If the agent gets close enough to the prey it kills it.
        -If the hunt takes too long, the agent will abandon the action.
    */
    private Agent targetAgent;
    private GameObject targetGameObject;
    private enum actionState{INACTIVE,CHASE, KILL,EAT, EXIT}
    private actionState currentState = actionState.INACTIVE;
    private List<Agent> nearbyPrey = new List<Agent>();
    private float maxChaseTime = 6, eatTimer =0, chaseTimer =0;
    private Vector2Int PreyPartitionCoords;
    private List<Vector2Int> pathToTarget = new List<Vector2Int>();
    private Vector3 pathStart;
    Vector3 targetLocation;
    private int pathIndex = 0;
    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "Hunt";
    }
    public override bool isActionPossible(PredatorDiscontents snapshot, bool isChainAction)
    {
        //If planning for the future, the agent assumes that it can find prey
        if(isChainAction){return true;}
        //Checks to see if the predator can see a prey
        List<Agent> nearbyAgents = GetVisibleAgents();
        foreach(Agent prey in nearbyAgents)
        {
            if(agent.GetPathfindingSystem().isTargetReachable(prey.GetGameObject().transform.position))
            {
                return true;
            }
        }
        return false;
    }
    public override float ActionScore(PredatorDiscontents snapshot, bool isChainAction)
    {
        return (snapshot.GetHunger() * snapshot.GetHunger() * 200);
    }
    public override float EstimatedDuration(PredatorDiscontents snapshot)
    {
        return 6;
    }
    public override void PerformAction()
    {
        //Get the nearest reachable agent
        float distance, closest = float.MaxValue;
        List<Agent> nearbyPrey = GetVisibleAgents();
        targetAgent = null;
        foreach(Agent prey in nearbyPrey)
        {
            //Skip if the agent cant be reached
            if(!agent.GetPathfindingSystem().isTargetReachable(prey.GetGameObject().transform.position, agent.GetGenome().visionRadius)){continue;}
            distance = Vector3.Distance(transform.position, prey.GetGameObject().transform.position);
            if(distance < closest)
            {
                closest = distance;
                targetAgent = prey;
            }
        }
        //Return if no valid agents
        if(targetAgent == null){return;}
        pathStart = PartitionSystem.instance.PartitionToWorldCoords(agent.GetCurrentPartition());
        PreyPartitionCoords = PartitionSystem.instance.WorldToPartitionCoords(targetAgent.GetGameObject().transform.position);
        pathToTarget = agent.GetPathfindingSystem().GetPathToPartition(PreyPartitionCoords, agent.GetGenome().visionRadius);
        pathIndex = 1;
        eatTimer = 0;
        chaseTimer = 0;
        currentState = actionState.CHASE;
        targetGameObject = targetAgent.GetGameObject();
        agent.SetPerformingAction(true);
    }
    public override void UpdateAction()
    {
        if(targetGameObject == null && currentState != actionState.EAT)
        {
            ExitAction();
            return;
        }
        switch(currentState)
        {
            case actionState.INACTIVE:
                return;
            case actionState.CHASE:
                ChaseTargetAgent();
                break;
            case actionState.KILL:
                KillTarget();
                break;
            case actionState.EAT:
                EatTarget();
                break;
            case actionState.EXIT:
                ExitAction();
                break;
        }
    }
    private void ChaseTargetAgent()
    {
        //Check if the predator has reached the preys partition
        Vector2Int coords = PartitionSystem.instance.WorldToPartitionCoords(targetAgent.GetGameObject().transform.position);
        Vector3 predatorCoords = PartitionSystem.instance.PartitionToWorldCoords(agent.GetCurrentPartition()); 
        if(agent.GetCurrentPartition() == coords)
        {
            currentState = actionState.KILL;
            return;
        }
        //Check if path is still valid
        if(coords != PreyPartitionCoords || predatorCoords != pathStart)
        {
            PreyPartitionCoords = coords;
            //Check if can still reach target agent
            if(!agent.GetPathfindingSystem().isTargetReachable(targetAgent.GetGameObject().transform.position))
            {
                currentState = actionState.EXIT;
                return;
            }
            //Create new path
            pathToTarget = agent.GetPathfindingSystem().GetPathToPartition(coords,agent.GetGenome().visionRadius);
            if(pathToTarget == null)
            {
                currentState = actionState.EXIT;
                return;
            }   
            pathIndex =1;
            pathStart = predatorCoords;
        }
        //move along path
        Vector2Int targetPartition = agent.GetCurrentPartition() + pathToTarget[pathIndex];
        targetLocation = PartitionSystem.instance.PartitionToWorldCoords(targetPartition);
        targetLocation.y = transform.position.y;
        Vector3 velocity = targetLocation - transform.position;
        velocity.y = 0;
        velocity.Normalize();
        agent.SetVelocity(velocity);
        if(Vector3.Distance(transform.position, targetLocation) < 0.2f)
        {
            Debug.Log("checkpoint reached");
            pathIndex++;
        }
    }
    private void KillTarget()
    {
        currentState = actionState.KILL;
        if(targetAgent != null)
        {
            targetAgent.Kill();
            currentState = actionState.EAT;
            return;
        }
        currentState = actionState.EXIT;
        return;
    }
    private void EatTarget()
    {
        currentState = actionState.EAT;
        agent.SetVelocity(Vector3.zero);
        targetGameObject = null;
        agent.SetEating(true);
        eatTimer+= Time.deltaTime;
        if(eatTimer >= 2)
        {
            agent.SetEating(false);
            currentState = actionState.EXIT;
            return;
        }
    }
    public override void ExitAction()
    {
        agent.SetEating(false);
        agent.SetPerformingAction(false);
        agent.SetVelocity(Vector3.zero);
        currentState = actionState.INACTIVE;
    }

    private List<Agent> GetVisibleAgents()
    {
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        List<Agent> agents = new List<Agent>();
        foreach(Partition partition in visiblePartitions)
        {
            if(partition.agents.Count == 0){continue;}
            foreach(GameObject targetAgent in partition.agents)
            {
                if(targetAgent == null){continue;}
                if(targetAgent == this.gameObject){continue;}
                if(targetAgent.GetComponent<Agent>().GetAgentType() == Agent.AgentType.PREY)
                {
                    agents.Add(targetAgent.GetComponent<Agent>());
                }
            }
        }
        return agents;
    }
}
