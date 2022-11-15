using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceAction : Action
{
    //Asexual reproduction action (cloning)

    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "Reproduce";
        agent = _agent;
        return agent.GetReproduction() == 1 ? true : false;
    }

    public override float ActionScore()
    {
        return agent.GetReproduction() == 1 ? 85 : 0;
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
        int gender = Random.Range(0,2);
        Material mat;
        if(gender == 0)
        {
            mat = EntitySpawner.instance.MaleMat;
        }
        else
        {
            mat = EntitySpawner.instance.FemaleMat;
        }
        GameObject prefab = EntitySpawner.instance.entityPrefab;
        GameObject childAgent = Instantiate(prefab, agent.transform.position, Quaternion.identity);
        childAgent.transform.parent = EntitySpawner.instance.transform;
        childAgent.GetComponent<MeshRenderer>().material = mat;
        Debug.Log("new agent spawned!");
        agent.SetReproduction(false);
        agent.SetPerformingAction(false);
        EntitySpawner.instance.currentPopulation++;
        yield return null;
    }
}
