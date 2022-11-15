using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAction : Action
{
    //Action that moves the AI in a random direction for a few seconds
    private float distance = 0;
    private Vector3 direction;
    public override bool isActionPossible(GOPAgent _agent)
    {
        actionName = "Wander";
        agent = _agent;
        //pick rand Direction
        int x = Random.Range(-10,10);
        int z = Random.Range(-10,10);
        direction = new Vector3(x,0,z);
        agent = _agent;
        RaycastHit hit;
        Vector3 startPos = agent.transform.position - Vector3.up * 0.1f;
        if(Physics.Raycast(startPos, startPos + direction * 10, out hit, 10))
        {
            distance = Vector3.Distance(agent.transform.position, hit.point);
            if(distance < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }
    public override float ActionScore()
    {
        return 15;
    }
    public override void PerformAction()
    {
        float speed = agent.GetSpeedModifier();
        direction.Normalize();
        agent.setVelocity(direction * speed);
        float duration = distance / speed;
        StartCoroutine(i_WanderTimer(duration));
    }
    private IEnumerator i_WanderTimer(float time)
    {
        agent.SetPerformingAction(true);
        yield return new WaitForSeconds(time);
        agent.SetPerformingAction(false);
        yield return null;
    }

}
