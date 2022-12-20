using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvSearch : AdvancedAction
{
    /*
        Advanced search action. This advanced action has the GOAP
        agent look around for prey or water, in the case they're hungry
        or thirsty but cant see any sources for food or water.
    */
    public override void Initialise(PredatorAgent _agent)
    {
        agent = _agent;
        actionName = "Search";
    }
    public override bool isActionPossible(PredatorDiscontents snapshot, bool isChainAction)
    {
        return base.isActionPossible(snapshot);
    }
    public override float ActionScore(PredatorDiscontents snapshot, bool isChainAction)
    {
        return base.ActionScore(snapshot);
    }
    public override float EstimatedDuration(PredatorDiscontents snapshot)
    {
        return base.EstimatedDuration(snapshot);
    }
    public override void PerformAction()
    {
        base.PerformAction();
    }
    public override void UpdateAction()
    {
        base.UpdateAction();
    }
    public override void ExitAction()
    {
        base.ExitAction();
    }
}
