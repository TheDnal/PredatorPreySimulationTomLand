using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAction : Action
{
    /*
        The Action class will attempt to have the agent sleep for a set duration
        This action is weighted by how tired the agent is.

    */
    #region Fields
    private float sleepTime = 2;
    private float timer = 0;
    private bool active = false;
    #endregion
    #region Inherrited methods
    public override bool isActionPossible(NewPreyAgent _agent)
    {
        actionName = "Sleep";
        agent = _agent;
        return agent.GetTiredness() > 0.33f ? true : false;
    }
    public override float ActionScore()
    {
        return agent.GetTiredness() * agent.GetTiredness() * 100;
    }
    public override void PerformAction()
    {
        timer = 0;
        agent.SetPerformingAction(true);
        agent.SetSleeping(true);
        agent.SetVelocity(Vector3.zero);
        active = true;
    }
    public override void UpdateAction()
    {
        if(!active)
        {
            return;
        }
        timer+= Time.deltaTime;
        if(timer >= sleepTime)
        {
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        agent.SetSleeping(false);
        agent.SetPerformingAction(false);
        active = false;
    }
    #endregion
}
