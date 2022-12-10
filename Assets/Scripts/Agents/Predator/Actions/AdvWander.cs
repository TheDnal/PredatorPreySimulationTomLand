using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvWander : AdvancedAction
{
    /*
        Advanced Wander action. This advanced action moves 
        the GOAP agent in a random direction for a 4 seconds.
    */
    private float timer = 0;
    private Vector3 wanderDirection = Vector3.zero;
    public override bool isActionPossible(PredatorDiscontentSnapshot snapshot){return true;}
    public override float ActionScore(PredatorDiscontentSnapshot snapshot){return 25;}
    public override float EstimatedDuration(){return 4;}
    public override void PerformAction()
    {
        actionRunning = true;
        agent.SetPerformingAction(true);
        float x = Random.Range(-10,10);
        float z = Random.Range(-10,10);
        wanderDirection = new Vector3(x,0,z);
        timer = 0;
        wanderDirection.Normalize();
    }
    public override void UpdateAction()
    {
        if(!actionRunning){return;}
        timer += Time.deltaTime;
        agent.SetVelocity(wanderDirection);
        if(timer >= 4)
        {
            agent.SetVelocity(Vector3.zero);
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        actionRunning = false;
        agent.SetPerformingAction(false);
    }
}
