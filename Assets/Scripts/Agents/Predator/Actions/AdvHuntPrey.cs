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
    private float maxChaseTime = 6, eatTimer =0, chaseTimer =0;

    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "Hunt";
    }
    public override bool isActionPossible(PredatorDiscontentSnapshot snapshot)
    {
        //Checks to see if the predator can see a prey
        return GetVisibleAgents().Count == 0 ? false : true;
    }
    public override float ActionScore(PredatorDiscontentSnapshot snapshot)
    {
        List<Agent> agents = GetVisibleAgents();
        float distance = 0, closest = float.MaxValue;
        targetAgent = null;
        foreach(Agent agent in agents)
        {
            if(agent == null){continue;}
            if(agent.GetAgentType() != Agent.AgentType.PREY){continue;}
            distance = Vector3.Distance(transform.position, agent.GetGameObject().transform.position);
            if(distance < closest)
            {
                targetAgent = agent;
                closest = distance;
            }
        }
        if(targetAgent == null)
        {
            return 0;
        }
        return (snapshot.GetHunger() * snapshot.GetHunger() * 200) - distance;
    }
    public override float EstimatedDuration(PredatorDiscontentSnapshot snapshot)
    {
        return base.EstimatedDuration(snapshot);
    }
    public override void PerformAction()
    {
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
        currentState = actionState.CHASE;
        if(targetGameObject == null)
        {
            currentState = actionState.EXIT;
            return;
        }
        float distance = Vector3.Distance(transform.position,targetGameObject.transform.position);
        if(distance < 0.33f)
        {
            currentState = actionState.KILL;
            return;
        }

        chaseTimer+= Time.deltaTime;
        if(chaseTimer >= maxChaseTime)
        {
            currentState = actionState.EXIT; 
            return;
        }
        
        Vector3 velocity = targetGameObject.transform.position - transform.position;
        velocity.y = 0;
        velocity.Normalize();
        agent.SetVelocity(velocity * 1.25f);
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
            foreach(GameObject agent in partition.agents)
            {
                if(agent.GetComponent<Agent>().GetAgentType() == Agent.AgentType.PREY)
                {
                    agents.Add(agent.GetComponent<Agent>());
                }
            }
        }
        return agents;
    }
}
