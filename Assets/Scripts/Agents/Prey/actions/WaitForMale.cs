using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForMale : Action
{
//  ▄██████▄   ▄█       ████████▄  
// ███    ███ ███       ███   ▀███ 
// ███    ███ ███       ███    ███ 
// ███    ███ ███       ███    ███ 
// ███    ███ ███       ███    ███ 
// ███    ███ ███       ███    ███ 
// ███    ███ ███▌    ▄ ███   ▄███ 
//  ▀██████▀  █████▄▄██ ████████▀  
//            ▀                    

    public override bool isActionPossible(PreyAgent _agent)
    {
        actionRunning = false;
        actionName = "Mating";
        agent = _agent;
        return agent.GetGender() == 1 ? true : false;
    }
    public override float ActionScore()
    {
        return 0;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
    }
}
