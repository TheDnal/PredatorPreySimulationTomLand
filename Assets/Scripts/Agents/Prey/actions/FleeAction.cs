using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAction : Action
{
    //   █████▒ ██▓    ▓█████ ▓█████ 
    // ▓██   ▒ ▓██▒    ▓█   ▀ ▓█   ▀ 
    // ▒████ ░ ▒██░    ▒███   ▒███   
    // ░▓█▒  ░ ▒██░    ▒▓█  ▄ ▒▓█  ▄ 
    // ░▒█░    ░██████▒░▒████▒░▒████▒
    //  ▒ ░    ░ ▒░▓  ░░░ ▒░ ░░░ ▒░ ░
    //  ░      ░ ░ ▒  ░ ░ ░  ░ ░ ░  ░
    //  ░ ░      ░ ░      ░      ░   
    //             ░  ░   ░  ░   ░  ░
    private Vector3 fearCentre = Vector3.up;
    private bool active = false;
    private float timer = 0;
    private Vector3 velocity;
    public override bool isActionPossible(PreyAgent _agent)
    {
        actionName = "Flee";
        agent = _agent;
        return agent.GetDanger() > 0.75 ? true : false;
    }
    public override float ActionScore()
    {
       return agent.GetDanger() * 200;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
        active = true;
        timer = 0;
        //Getting the most dangerous partition
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        Partition mostDangerous = null;
        float maxDanger = 0;
        foreach(Partition partition in visiblePartitions)
        {
            if(partition.GetDangerValue(true) == 0)
            {
                continue;
            }
            float danger = partition.GetDangerValue(true) * (1 / Vector3.Distance(transform.position, partition.worldPosition));
            if(danger > maxDanger)
            {
                maxDanger = danger;
                mostDangerous = partition;
                fearCentre = partition.worldPosition;
            }
        }
        if(maxDanger == 0)
        {
            Debug.Log("no danger");
            ExitAction();
            return;
        }
    }
    public override void UpdateAction()
    {
        if(!active || fearCentre == Vector3.up){return;}
        velocity = transform.position - fearCentre;
        velocity.y = 0;
        velocity.Normalize();
        agent.SetVelocity(velocity);
        timer+= Time.deltaTime;
        if(timer > 2)
        {
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        active = false;
        agent.SetPerformingAction(false);
    }
    void OnDrawGizmos()
    {
        if(active && fearCentre != Vector3.up)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(fearCentre, Vector3.one);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + velocity);
        }
    }
}
