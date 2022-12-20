using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvCallForMate : AdvancedAction
{
    private float timer = 0;
    private bool actionActive = false;
    public override void Initialise(PredatorAgent _agent)
    {
        actionName = "CallingForMate";
        agent = _agent;
    }
    public override bool isActionPossible(PredatorDiscontents snapshot, bool isChainAction)
    {
        //Agent cannot call if hungry, tired, thirsty, in danger etc
        if(snapshot.GetHunger() > 0.5f || agent.GetThirst() > 0.5f || agent.GetTiredness() > 0.5f || agent.GetDanger() > 0 || agent.GetReproductiveUrge() < 0.5)
        {
            return false;
        }
        //If planning for the future, the agent cannot assume it'll hear mates therefore it'll signal itself
        if(isChainAction){return true;}
        //If the agent can hear a call for mate, return false
        Vector2Int localPartitionCoords = agent.GetCurrentPartition();
        int gender = agent.GetGender();
        Partition localPartition = PartitionSystem.instance.partitions[localPartitionCoords.x, localPartitionCoords.y];
        foreach(noise currentNoise in localPartition.noises)
        {
            if(currentNoise.type == noise.noiseType.femalePredatorMatingCall && gender == 0)
            {
                return false;
            }
            if(currentNoise.type == noise.noiseType.malePredatorMatingCall && gender == 1)
            {
                return false;
            }
        }
        return true;
    }
    public override float ActionScore(PredatorDiscontents snapshot, bool isChainAction)
    {
        return agent.GetReproductiveUrge() * agent.GetReproductiveUrge() * 160;
    }
    public override float EstimatedDuration(PredatorDiscontents snapshot)
    {
        return 4;
    }
    public override void PerformAction()
    {
        timer = 0;
        actionActive = true;
        agent.SetPerformingAction(true);
        //Emit mating call
        if(agent.GetGender() == 0)
        {
            PartitionSystem.instance.EmitSound(5, transform.position, noise.noiseType.malePredatorMatingCall);
        }
        else
        {
            PartitionSystem.instance.EmitSound(5, transform.position, noise.noiseType.femalePredatorMatingCall);
        }
    }
    public override void UpdateAction()
    {
        if(!actionActive){return;}
        agent.SetVelocity(Vector3.zero);
        timer += Time.deltaTime;
        if(timer > 4){ExitAction();}
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        actionActive = false;
    }
    public override bool isActionChainable()
    {
        return true;
    }
}
