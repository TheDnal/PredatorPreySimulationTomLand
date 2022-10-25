using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyAgent : GOPAgent
{
    //GOPAgent that is a herbavore and avoids predators
    void Start()
    {
        Initialise();
    }

    public override void Initialise()
    {
        base.Initialise();
        WanderAction wander = this.gameObject.AddComponent<WanderAction>();
        actions.Add(wander);
    }
    void Update()
    {
        rb.velocity = velocity;
        if(performingAction)
        {
            return;
        }
        Action bestAction = CalculateBestAction();
        bestAction.PerformAction();
    }
}
