using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAction : Action
{                                        
    /*
        This Action class will attempt to move the agent into a random direction
        Note that this random direction is actually weighted. The partition system
        is kind enough to have a real-time score of each partition zone. If the agent 
        find itself hungry or thirsty with no access to these, it can get a helping 
        hand from the partition system and get a hint towards which partitions to move towards

        This action is broken down into the following steps:
        1) Is the agent eligible to get a hint from the partition system? If so, get a some scores
        and move towards the best one (TODO)
        2) if not, move in a random direction
        3) continue until either hitting an obstacle or ran out of time
        4) return control to the agent
    */

    #region Fields
    private Vector3 direction;
    private float distance;
    private Vector3 endPoint;
    private bool active = false;
    private float timer = 0;
    private float wanderDuration = 0;
    private Vector3 previousPosition;
    #endregion
    #region Inherrited methods
    public override bool isActionPossible(NewPreyAgent _agent){agent = _agent; actionName = "Wander"; return true;}
    public override float ActionScore()
    {
        return 20;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
        active = true;
        timer = 0;
        wanderDuration = 0;
        previousPosition = transform.position;
        float x =Random.Range(-5,5);
        float z = Random.Range(-5,5);
        direction = new Vector3(x,0,z);
        direction.Normalize();
    }
    public override void UpdateAction()
    {
        if(!active)
        {
            return;
        }
        timer += Time.deltaTime;
        wanderDuration += Time.deltaTime;
        if(wanderDuration >= 3)
        {
            ExitAction();
            return;
        }
        //Has the agent moved far since last interval? If not, they're stuck and should move on
        if(timer >= .25f)
        {
            timer = 0;
            float distance = Vector3.Distance(previousPosition, transform.position);
            previousPosition = transform.position;
            if(distance < 0.05f)
            {
                ExitAction();
                return;
            }
        }

        //Move the agent
        agent.SetVelocity(direction);
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        active = false;
    }
    #endregion
    #region Action states
    public void MoveInDirection()
    {

    }
    #endregion
    #region Misc
    //TODO
    private bool EligibleForHint()
    {
        return false;
    }
    #endregion

}
