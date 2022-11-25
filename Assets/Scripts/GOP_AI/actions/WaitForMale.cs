using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForMale : Action
{
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionRunning = false;
        actionName = "Mating";
        agent = _agent;
        return agent.validFemale;
    }
    public override float ActionScore()
    {
        return agent.suitableMale ? 999 : 0;
    }
    public override void PerformAction()
    {
        StartCoroutine(i_WaitForMale());
    }
    private IEnumerator i_WaitForMale()
    {
        actionRunning = true;
        agent.SetPerformingAction(true);
        while(!agent.pregnant || agent.getMaleMate() != null)
        {
            yield return new WaitForEndOfFrame();
        }
        agent.SetPerformingAction(false);
    }
}
