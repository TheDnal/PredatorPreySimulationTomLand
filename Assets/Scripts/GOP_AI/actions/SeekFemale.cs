using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekFemale : Action
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

    private GameObject nearestFemale;
    public override bool isActionPossible(NewPreyAgent _agent)
    {
        actionRunning = false;
        actionName = "Mating";
        agent = _agent;
        List<Partition> visiblePartitions = agent.GetSensorySystem().GetVisionCone();
        bool canSeeFemale = false;
        foreach(Partition p in visiblePartitions)
        {
            if(p.agents.Count > 0)
            {
                foreach(GameObject agent in p.agents)
                {
                    if(agent.GetComponent<GOPAgent>().GetGender() == 1 && agent.GetComponent<GOPAgent>().validFemale)
                    {
                        nearestFemale = agent;
                        canSeeFemale = true;
                        break;
                    }
                }
            }
        }
        return canSeeFemale;
    }
    public override float ActionScore()
    {
        if(nearestFemale == null)
        {
            return 0;
        }
        return agent.GetReproductiveUrge() > 0.6 ? agent.GetReproductiveUrge() * 150 : 0;
    }
    public override void PerformAction()
    {
        // agent.SetPerformingAction(true);
        // if(PartitionSystem.instance.WorldToPartitionCoords(nearestFemale.transform.position) == agent.getCurrPartition())
        // {
        //     MateWithFemale();
        // }
        // if(!agent.isTargetReachable(nearestFemale.transform.position))
        // {
        //     agent.SetPerformingAction(false);
        //     return;
        // }
        // actionRunning = true;
        // agent.PathToTarget(nearestFemale.transform.position);
        // StartCoroutine(i_WaitUntilReachedFemale());
    }
    // private IEnumerator i_WaitUntilReachedFemale()
    // {
    //     // while(!agent.arrivedAtDestination)
    //     // {
    //     //     if(nearestFemale == null)
    //     //     {
    //     //         actionRunning = false;
    //     //         yield return null;
    //     //     }
    //     //     yield return new WaitForEndOfFrame();
    //     // }
    //     // agent.arrivedAtDestination = false;
    //     // MateWithFemale();
    //     // agent.SetPerformingAction(false);
    // }
    public void MateWithFemale()
    {
        if(nearestFemale != null && nearestFemale.GetComponent<GOPAgent>().validFemale)
        {
            agent.ResetReproduction();
            nearestFemale.GetComponent<GOPAgent>().Mate();
        }
    }
}
