using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvGiveBirth : AdvancedAction
{
    float timer = 0;
    int offspringCount = 0;
    int maxOffspring = 2;
    float spawnInterval = 1;
    public override void Initialise(PredatorAgent _agent)
    {
        actionName = "Reproduce";
        agent = _agent;
    }
    public override bool isActionPossible(PredatorDiscontentSnapshot snapshot)
    {
        return snapshot.GetPregnancy() >= 1 ? true : false;
    }
    public override float ActionScore(PredatorDiscontentSnapshot snapshot)
    {
        return 999;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
        timer = 0;
        offspringCount = 0;
        agent.SetVelocity(Vector3.zero);
    }
    public override void UpdateAction()
    {
        timer += Time.deltaTime;
        if(timer < spawnInterval)
        {
            return;
        }
        if(offspringCount == maxOffspring)
        {
            ExitAction();
            return;
        }
        SpawnChild();  
    }
    public void SpawnChild()
    {
        Debug.Log("child predator born!");
        offspringCount ++;
        agent.IncrementOffspringCount();
        int gender = Random.Range(0,2);
        GameObject prefab = EntitySpawner.instance.predatorPrefab;
        Vector3 pos = transform.position;
        pos += new Vector3(Random.Range(-0.1f,0.1f), 0.1f, Random.Range(-0.1f,0.1f));
        GameObject childAgent = Instantiate(prefab, pos, Quaternion.identity);
        childAgent.GetComponent<PredatorAgent>().Initialise(gender, agent.GetGenome());
        EntitySpawner.instance.currentPopulation++;
        childAgent.transform.parent = this.transform.parent;
        if(gender == 0)
        {
            childAgent.GetComponent<MeshRenderer>().material = EntitySpawner.instance.MalePredatorMat;
        }  
        else
        {
            childAgent.GetComponent<MeshRenderer>().material = EntitySpawner.instance.FemalePredatorMat;
        }
    }
    public override void ExitAction()
    {
        agent.ResetPregnancy();
        agent.SetPerformingAction(false);
    }
}
