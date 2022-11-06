using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    /*
      Template class to be used to construct 
      actions for GOP (Goal oriented planner)
      Actions represent the different behaviours the 
      GOP-AI can perform. Actions have a score, with
      the action with the hightest score being
      performed by the GOP-AI. 
    */

    //Inherited Fields
    #region Fields
    protected GOPAgent agent;
    protected bool actionRunning = false;
    #endregion


    //Inherited Methods
    #region Methods 
    public virtual bool isActionPossible(GOPAgent _agent){return false;}
    public virtual float ActionScore(){return 0;}
    public virtual void PerformAction(){}
    #endregion
}
