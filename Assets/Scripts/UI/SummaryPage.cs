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
    private GameObject selectedGameObject;
    public override void InitialisePage()
    {
        gameObject.SetActive(true);
        if(Agent.selectedAgent == null)
        {
            Debug.Log("ding!");
            agentType.text           = "Agent type:";
            agentAge.text            = "Agent age";
            agentOffspringCount.text = "no. Offspring:";
            agentGender.text         = "Gender :";
            return;
        }
        agentType.text           = "Agent type: \n" + Agent.selectedAgent.GetAgentType();
        agentAge.text            = "Agent age:";
        agentOffspringCount.text = "no. Offspring:";
        agentGender.text         = Agent.selectedAgent.GetGender() == 0 ? "Gender : \n Male" : "Gender \n Female";
    }
    public override void UpdatePage()
    {
        if(Agent.selectedAgent == null)
        {
            agentType.text           = "Agent type:";
            agentAge.text            = "Agent age";
            agentOffspringCount.text = "no. Offspring:";
            agentGender.text         = "Gender :";
            followAgent = false;
            return;
        }
        selectedGameObject = Agent.selectedAgent.GetGameObject();
        _age                     = Agent.selectedAgent.GetAge();
        offspringCount           = Agent.selectedAgent.GetOffspringCount();
        agentAge.text            = "Agent age: \n" + _age;
        agentOffspringCount.text = "no. Offspring: \n" + offspringCount;
        agentGender.text         = Agent.selectedAgent.GetGender() == 0 ? "Gender : \n Male" : "Gender \n Female";

        if(followAgent)
        {
            Vector3 pos = mainCam.transform.position;
            pos.x = selectedGameObject.transform.position.x;
            pos.z = selectedGameObject.transform.position.z;
            mainCam.transform.position = pos;
        }
    }
    public void ToggleFollowingAgent()
    {
        followAgent = followAgent ? false : true;
    }
}
