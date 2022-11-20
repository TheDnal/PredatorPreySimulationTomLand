using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAction : Action
{
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "Sleep";
        agent = _agent;
        return true;
    }
    public override float ActionScore()
    {
        return agent.GetTiredness() > 0.75f ? agent.GetTiredness() * 100 : 0;
    }
    public override void PerformAction()
    {
        Sleep();
    }
    private void Sleep()
    {
        StartCoroutine(i_Sleep());
    }
    private IEnumerator i_Sleep()
    {
        agent.SetPerformingAction(true);
        agent.SetSleeping(true);
        yield return new WaitForSeconds(2f);
        agent.SetSleeping(false);
        agent.SetPerformingAction(false);
    }
}
