using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatingCall : Action
{
    float timer = 0;
    bool actionActive = false;
    public override bool isActionPossible(PreyAgent _agent)
    {
        agent = _agent;
        if(agent.GetHunger() > 0.75 || agent.GetThirst() > 0.75 || agent.GetTiredness() > 0.75) { return false; }
        return agent.GetReproductiveUrge() > 0.5 ? true : false;
    }
    public override float ActionScore()
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 85;
    }
    public override void PerformAction()
    {
        //Emit mating call
        agent.SetPerformingAction(true);
        timer = 0;
        actionActive = true;
        PartitionSystem.instance.EmitSound(2, transform.position, noise.noiseType.matingCall);
    }
    public override void UpdateAction()
    {
        if (!actionActive) { return; }
        timer += Time.deltaTime;
        if (timer >= 4)
        {
            ExitAction();
        }
        
    }
    public override void ExitAction()
    {
        actionActive = false;
        agent.SetPerformingAction(false);
    }
}
