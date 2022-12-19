using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAction : MonoBehaviour
{
    /*
        Template class used to construct 
        actions for GOAP (Goal oriented action planners).
        Advanced actions represent the behaviors GOAP can
        exhibit, but also allow for the GOAP to chain together 
        these actions into a plan.
    */
    #region Variables
    protected PredatorAgent agent;
    protected string actionName;
    protected bool actionRunning = false;
    #endregion
    #region Generic Methods
    public virtual void Initialise(PredatorAgent _agent){agent = _agent;}
    public virtual bool isActionPossible(PredatorDiscontentSnapshot snapshot){return false;}
    public virtual bool isActionChainable(){return false;}
    public virtual float ActionScore(PredatorDiscontentSnapshot snapshot){return 0;}
    public virtual float EstimatedDuration(PredatorDiscontentSnapshot snapshot){return 0;}
    public virtual void PerformAction(){}
    public virtual void UpdateAction(){}
    public virtual void ExitAction(){}
    public bool IsActionRunning(){return actionRunning;}
    public string GetActionName(){return actionName;}
    #endregion
}
