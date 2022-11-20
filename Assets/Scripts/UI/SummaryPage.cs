using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SummaryPage : InspectorPage
{
    public TextMeshProUGUI agentType;
    public TextMeshProUGUI agentAge;
    public TextMeshProUGUI agentOffspringCount;
    private float _age, offspringCount;
    public override void InitialisePage(EntityInspector _inspector)
    {
        base.InitialisePage(_inspector);
        if(inspector.currentEntity == null)
        {
            return;
        }
        currentAgent             = inspector.currentEntity.GetComponent<GOPAgent>();
        agentType.text           = "Agent type: \n" + currentAgent.agentType;
        agentAge.text            = "Agent age:";
        agentOffspringCount.text = "no. Offspring:";
    }
    public override void UpdatePage()
    {
        if(currentAgent == null)
        {
            return;
        }
        _age                     = currentAgent.age;
        offspringCount           = currentAgent.offspring;
        agentAge.text            = "Agent age: \n" + _age;
        agentOffspringCount.text = "no. Offspring: \n" + offspringCount;
    }
}
