using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvSleep : AdvancedAction
{
    /*
        Advanced sleep action. This action allows the 
        GOAP agent to rest for 4 seconds
    */
    private float timer = 0;
    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "Sleep";
    }
    public override bool isActionPossible(PredatorDiscontents snapshot, bool isChainAction)
    {
        return snapshot.GetTiredness() > 0.5 ? true : false;
    }
    public override float ActionScore(PredatorDiscontents snapshot, bool isChainAction)
    {
        return snapshot.GetTiredness() * snapshot.GetTiredness() * 75;
    }
    public override float EstimatedDuration(PredatorDiscontents snapshot)
    {
        return 4;
    }
    public override void PerformAction()
    {
        agent.SetSleeping(true);
        timer = 0;
    }
    public override void UpdateAction()
    {
        timer += Time.deltaTime;
        if(timer >= 4)
        {
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        agent.SetSleeping(false);
    }
    public override bool isActionChainable()
    {
        return true;
    }
}
