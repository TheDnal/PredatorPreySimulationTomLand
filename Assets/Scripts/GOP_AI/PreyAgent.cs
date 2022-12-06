using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyAgent : GOPAgent
{
    //GOPAgent that is a herbavore and avoids predators

    private PartitionSystem pSystem;
    private List<Partition> adjacentPartitions = new List<Partition>();
    private bool initialised = false;
    private Action bestAction;
    void Start()
    {
        currPartition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
        pathfindingSystem.Initialise(this);
        agentType = "Prey agent";
        Initialise();
    }

    public override void Initialise()
    {
        base.Initialise();
        showGizmos = false;
        currentActionName = "null";
        pSystem = PartitionSystem.instance;
        currPartition = pSystem.WorldToPartitionCoords(transform.position);
        pSystem.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.agent);

        WanderAction wander = this.gameObject.AddComponent<WanderAction>();
        actions.Add(wander);

        GetFoodAction getFoodAction = this.gameObject.AddComponent<GetFoodAction>();
        actions.Add(getFoodAction);

        GetWaterAction getWaterAction = this.gameObject.AddComponent<GetWaterAction>();
        actions.Add(getWaterAction);

        FleeAction flee = this.gameObject.AddComponent<FleeAction>();
        actions.Add(flee);
        if(gender == 0)
        {
            // SeekFemale seekFemale = this.gameObject.AddComponent<SeekFemale>();
            // actions.Add(seekFemale);
        }
        else
        {
            // WaitForMale waitForMale = this.gameObject.AddComponent<WaitForMale>();
            // actions.Add(waitForMale);
            // ReproduceAction reproduceAction = this.gameObject.AddComponent<ReproduceAction>();
            // actions.Add(reproduceAction);
        }

        SleepAction sleepAction = this.gameObject.AddComponent<SleepAction>();
        actions.Add(sleepAction);

        initialised = true;
    }
    void Update()
    {
        if(!initialised)
        {
            return;
        }
        if(transform.position.y < 0)
        {
            Debug.Log("Agent fell out of bounds, killing agent");
            killAgent();
        }
        UpdateDiscontent();
        Vector2Int oldPartitionPos = currPartition;
        currPartition = pSystem.WorldToPartitionCoords(transform.position);

        if(oldPartitionPos != currPartition)
        {
            pSystem.RemoveGameObjectFromPartition(this.gameObject, oldPartitionPos, PartitionSystem.ObjectType.agent);
            pSystem.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.agent);
            adjacentPartitions = pSystem.GetPartitionsInRadius(transform.position, 2);
        }
        transform.LookAt(transform.position + velocity);
        rb.velocity = velocity;
        if(performingAction)
        {
            if(bestAction != null)
            {
                bestAction.UpdateAction();
            }
            return;
        }
        bestAction = CalculateBestAction();
        currentActionName = bestAction.actionName;
        bestAction.PerformAction();
    }

    void OnMouseDown()
    {
        EntityInspector.instance.SetSelectedEntity(this.gameObject, EntityInspector.EntityType.prey);
    }
}

