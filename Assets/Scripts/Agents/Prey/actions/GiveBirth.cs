using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBirth : Action
{
    private float timer = 0;
    private float spawnInterval = 1;
    private int maxOffspring = 2, offspringCount = 0;
    public override bool CanActionOverrideOthers()
    {
        actionName = "Reproduce";
        return true;
    }
    public override bool isActionPossible(PreyAgent _agent)
    {
        agent = _agent;
        return agent.GetPregnancy() >= 1 ? true : false;
    }
    public override float ActionScore()
    {
        return 999;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
        timer = 0;
        offspringCount = 0;
    }
    public override void UpdateAction()
    {
        timer+= Time.deltaTime;
        if(timer > spawnInterval)
        {
            timer =0;
            if(offspringCount == maxOffspring)
            {
                ExitAction();
                return;
            }
            SpawnChild();
        }
    }
    public void SpawnChild()
    {
        offspringCount ++;
        agent.IncrementOffspringCount();
        int gender = Random.Range(0,2);
        GameObject prefab = EntitySpawner.instance.preyPrefab;
        Vector3 pos = transform.position;
        pos += new Vector3(Random.Range(-0.1f,0.1f),0.1f,Random.Range(-0.1f,0.1f));
        GameObject childAgent = Instantiate(prefab, pos, Quaternion.identity);
        childAgent.GetComponent<PreyAgent>().Initialise(gender, agent.GetGenome());
        EntitySpawner.instance.currentPopulation++;
        EntitySpawner.instance.AddEntity(childAgent.gameObject);
        childAgent.transform.parent = this.transform.parent;
    }
    public override void ExitAction()
    {
        agent.ResetPregnancy();
        agent.SetPerformingAction(false);
    }
}
