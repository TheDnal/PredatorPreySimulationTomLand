using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SummaryPage : InspectorPage
{
    public TextMeshProUGUI agentType;
    public TextMeshProUGUI agentAge;
    public TextMeshProUGUI agentGender;
    public TextMeshProUGUI agentOffspringCount;
    private float _age, offspringCount;
    private bool followAgent = false;
    public GameObject mainCam;
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
        agentGender.text         = inspector.currentEntity.GetComponent<GOPAgent>().GetGender() == 0 ? "Gender : \n Male" : "Gender \n Female";
    }
    public override void UpdatePage()
    {
        if(currentAgent == null)
        {
            followAgent = false;
            return;
        }
        _age                     = currentAgent.age;
        offspringCount           = currentAgent.offspring;
        agentAge.text            = "Agent age: \n" + _age;
        agentOffspringCount.text = "no. Offspring: \n" + offspringCount;
        if(followAgent)
        {
            Vector3 pos = mainCam.transform.position;
            pos.x = currentAgent.transform.position.x;
            pos.z = currentAgent.transform.position.z;
            mainCam.transform.position = pos;
        }
    }
    void Update()
    {
        if(followAgent)
        {
            Vector3 pos = mainCam.transform.position;
            pos.x = currentAgent.transform.position.x;
            pos.z = currentAgent.transform.position.z;
            mainCam.transform.position = pos;
        }
    }
    public void ToggleFollowingAgent()
    {
        followAgent = followAgent ? false : true;
    }
}
