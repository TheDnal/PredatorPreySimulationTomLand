using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatingCall : Action
{
    float timer = 0;
    bool actionActive = false;
    public override bool isActionPossible(PreyAgent _agent)
    {
        actionName = "CallingForMate";
        agent = _agent;
        //Agent is locked out of this action if too hungry, tired, thirsty
        if(agent.GetHunger() > 0.5 || agent.GetThirst() > 0.5 || agent.GetTiredness() > 0.5) { return false; }

        //Return false if agent doesn't have enough urge
        if(agent.GetReproductiveUrge() < 0.5){return false;}

        //If the agent can hear mating calls from the right sex, it wont call itself
        int gender = agent.GetGender();
        Vector2Int currPartition = agent.GetCurrentPartition();
        Partition partition = PartitionSystem.instance.partitions[currPartition.x, currPartition.y];
        foreach (noise currentNoise in partition.noises)
        {
            if (currentNoise.type == noise.noiseType.femalePreyMatingCall && gender == 0)
            {
                return false;
            }
            else if(currentNoise.type == noise.noiseType.malePreyMatingCall && gender == 1)
            {
                return false;
            }
        }
        return true;
    }
    public override float ActionScore()
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 75;
    }
    public override void PerformAction()
    {
        //Emit mating call
        agent.SetPerformingAction(true);
        timer = 0;
        actionActive = true;
        if(agent.GetGender() == 0)
        {
            PartitionSystem.instance.EmitSound(2, transform.position, noise.noiseType.malePreyMatingCall);
        }
        else
        {
            PartitionSystem.instance.EmitSound(2, transform.position, noise.noiseType.femalePreyMatingCall);
        }
       
    }
    public override void UpdateAction()
    {
        if (!actionActive) { return; }
        timer += Time.deltaTime;
        if (timer >= 4)
        {
            ExitAction();
        }
        
    }
    public override void ExitAction()
    {
        actionActive = false;
        agent.SetPerformingAction(false);
    }
}
