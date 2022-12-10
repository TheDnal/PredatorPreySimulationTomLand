using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMateAction : Action
{
    private float timer = 0;
    private float wanderDuration = 3;
    private float lookDuration = 2;
    private float startRotation = 0;
    private float degrees = 0;
    private Vector3 searchDirection = Vector3.zero;
    public override bool isActionPossible(PreyAgent _agent){actionName = "FindMate"; agent = _agent; return true;}
    public override float ActionScore()
    {
        if(agent.GetHunger() > 0.5 || agent.GetThirst() > 0.5 || agent.GetTiredness() > 0.5){return 0;}
        float score = agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 100;
        return score > 25 ? score : 0;
    }
    public override void PerformAction()
    {
        agent.SetPerformingAction(true);
        agent.SetVelocity(Vector3.zero);
        startRotation = agent.transform.rotation.eulerAngles.y;
        degrees = 0;
        timer = 0;
        int x = Random.Range(-10,10);
        int z = Random.Range(-10,10);
        searchDirection.x = x;
        searchDirection.z = z;
        searchDirection.y = 0;
        searchDirection.Normalize();
    }
    public override void UpdateAction()
    {
        if(degrees < 360)
        {
            degrees += Time.deltaTime * (360 / lookDuration);
            Vector3 rot = agent.transform.rotation.eulerAngles;
            rot.y = degrees;
            agent.transform.rotation = Quaternion.Euler(rot);
            return;
        }
        timer += Time.deltaTime;
        if(timer > 3)
        {
            ExitAction();
        }
        agent.SetVelocity(searchDirection);
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
    }
}
