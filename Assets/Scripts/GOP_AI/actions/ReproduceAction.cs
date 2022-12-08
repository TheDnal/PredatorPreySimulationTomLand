using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceAction : Action
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
    private float timer = 0;
    private float birthInterval = 1;
    private int maxChildCount = 3;
    private int childCount = 0;
    public override bool isActionPossible(NewPreyAgent _agent)
    {
        actionName = "Reproduce";
        agent = _agent;
        
        return agent.GetPregnancy() >= 1 ? true : false;
    }

    public override float ActionScore()
    {
        return agent.GetPregnancy() >= 1 ? 999 : 0;
    }

    public override void PerformAction()
    {
        timer = 0;
        childCount = 0;
        agent.SetPerformingAction(true);
        StartCoroutine(i_Reproduce());
    }
    public override void UpdateAction()
    {
        if(childCount == maxChildCount)
        {
            ExitAction();
            return;
        }
        timer += Time.deltaTime;
        if(timer >= birthInterval)
        {
            childCount ++;
            timer = 0;
            Material mat;
            int gender = Random.Range(0,2);
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
        }
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
    }
    private IEnumerator i_Reproduce()
    {
        agent.SetPerformingAction(true);
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
            EntitySpawner.instance.currentPopulation++;
            EntitySpawner.instance.AddEntity(childAgent.gameObject);
        }
        agent.SetPerformingAction(false);
    }
}
