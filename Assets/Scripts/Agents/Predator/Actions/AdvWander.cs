using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvWander : AdvancedAction
{
    /*
        Advanced Wander action. This advanced action moves 
        the GOAP agent in a random direction for a 2 seconds.
    */
    private float timer = 0, stuckTimer = 0;
    private Vector3 previousPosition;
    private Vector3 wanderDirection = Vector3.zero;
    public override void Initialise(PredatorAgent _agent){agent = _agent; actionName = "Wander";}
    public override bool isActionPossible(PredatorDiscontentSnapshot snapshot){return true;}
    public override float ActionScore(PredatorDiscontentSnapshot snapshot){return 5;}
    public override float EstimatedDuration(PredatorDiscontentSnapshot snapshot){return 4;}
    public override void PerformAction()
    {
        actionRunning = true;
        agent.SetPerformingAction(true);
        float x = Random.Range(-10,10);
        float z = Random.Range(-10,10);
        wanderDirection = new Vector3(x,0,z);
        timer = 0;
        previousPosition = transform.position;
        wanderDirection.Normalize();
    }
    public override void UpdateAction()
    {
        if(!actionRunning){return;}
        timer += Time.deltaTime;
        stuckTimer += Time.deltaTime;
        agent.SetVelocity(wanderDirection);
        if(timer >= 2)
        {
            agent.SetVelocity(Vector3.zero);
            ExitAction();
        }
        //Has the agent moved far since last interval? If not, they're stuck and should move on
        if(stuckTimer >= .25f)
        {
            stuckTimer = 0;
            float distance = Vector3.Distance(previousPosition, transform.position);
            previousPosition = transform.position;
            if(distance < 0.05f)
            {
                ExitAction();
                return;
            }
        }
    }
    public override void ExitAction()
    {
        agent.SetVelocity(Vector3.zero);
        actionRunning = false;
        agent.SetPerformingAction(false);
    }
}
