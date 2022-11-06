using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceAction : Action
{
    //Asexual reproduction action (cloning)

    public override bool isActionPossible(GOPAgent _agent)
    {
        agent = _agent;
        return agent.GetReproduction() == 1 ? true : false;
    }

    public override float ActionScore()
    {
        return agent.GetReproduction() == 1 ? 50 : 0;
    }

    public override void PerformAction()
    {
        StartCoroutine(i_Reproduce());
    }

    private IEnumerator i_Reproduce()
    {
        agent.SetPerformingAction(true);
        agent.SetReproduction(true);
        yield return new WaitForSeconds(1f);
        GameObject prefab = EntitySpawner.instance.entityPrefab;
        GameObject daughterAgent = Instantiate(prefab, agent.transform.position, Quaternion.identity);
        daughterAgent.transform.parent = EntitySpawner.instance.transform;
        Debug.Log("new agent spawned!");
        agent.SetReproduction(false);
        agent.SetPerformingAction(false);
        yield return null;
    }
}
