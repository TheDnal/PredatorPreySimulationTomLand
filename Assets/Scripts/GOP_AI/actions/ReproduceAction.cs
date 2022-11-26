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
        
        return agent.pregnancy >= 1 ? true : false;
    }

    public override float ActionScore()
    {
        return agent.pregnancy >= 1 ? 999 : 0;
    }

    public override void PerformAction()
    {
        StartCoroutine(i_Reproduce());
    }

    private IEnumerator i_Reproduce()
    {
        agent.SetPerformingAction(true);
        agent.SetReproduction(true);
        for(int i =0; i < 3; i++)
        {
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
            GameObject childAgent = Instantiate(prefab, this.transform.position, Quaternion.identity);
            childAgent.transform.parent = EntitySpawner.instance.transform;
            childAgent.GetComponent<GOPAgent>().SetGender(gender);
            childAgent.GetComponent<MeshRenderer>().material = mat;
            agent.offspring++;
            EntitySpawner.instance.currentPopulation++;
        }
        agent.pregnant = false;
        agent.validFemale = true;
        agent.SetReproduction(false);
        agent.SetPerformingAction(false);
    }
}
