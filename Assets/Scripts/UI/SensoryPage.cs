using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryPage : InspectorPage
{
    public enum Sense{sight = 1,sound = 2,smell = 3,none = 0};
    bool displayGizmos = false;
    public Sense currSense = Sense.none;
    private List<Partition> VisionCone = new List<Partition>();
    private List<Partition> smellPartitions = new List<Partition>();
    private List<Partition> displayedPartitions = new List<Partition>();
    public override void InitialisePage(EntityInspector _Inspector)
    {
        base.InitialisePage(_Inspector);
    }
    public override void UpdatePage()
    {
        base.UpdatePage();
        VisionCone = inspector.currentEntity.GetComponent<SVision>().GetVisionCone();
        smellPartitions = inspector.currentEntity.GetComponent<SVision>().GetSmell();
    }
    public void SetCurrentSense(Sense currentSense)
    {
        currSense = currentSense;
    }
    public void SetCurrentSense(int index)
    {
        displayGizmos = true;
        if((Sense)index == currSense)
        {
            currSense = Sense.none;
            displayGizmos = false;
            return;
        }
        currSense = (Sense)index;
    }
    public override void ClosePage()
    {
        displayGizmos = false;
    }
    void OnDrawGizmos()
    {
        if(!displayGizmos)
        {
            return;
        }
        switch(currSense)
        {
            case Sense.sight:
                Gizmos.color = Color.white;
                displayedPartitions = VisionCone;
                break;
            case Sense.sound:
                break;
            case Sense.smell:
                Gizmos.color = Color.green;
                displayedPartitions = smellPartitions;
                break;
            default:
                break;
        }
        if(displayedPartitions.Count == 0)
        {
            return;
        }
        foreach(Partition p in displayedPartitions)
        {
            Gizmos.DrawWireCube(p.worldPosition + Vector3.up, Vector3.one);
        }
    }
}
