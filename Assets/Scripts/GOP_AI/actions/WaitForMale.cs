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
        agent.SetPerformingAction(true);
        StartCoroutine(i_WaitForMale());
    }
    private IEnumerator i_WaitForMale()
    {
        actionRunning = true;
        float x = 0;
        while(!agent.pregnant || agent.getMaleMate() != null)
        {
            x+= Time.deltaTime;
            if(x > 5)
            {
                Debug.Log("Female lost interest");
                agent.SetPerformingAction(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        agent.SetPerformingAction(false);
    }
}
