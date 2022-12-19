using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BrainPage : InspectorPage
{
    [Header("Action images")]
    public Image actionImage;
    public Sprite Eat,Drink,Wander,Mating,Reproduce,Sleep, Flee, movingToMate, callingMates, Hunt, NULL, clear;
    public TMPro.TextMeshProUGUI text;
    [Space(6),Header("Graph Images")]
    public Image Hunger;
    public Image Thirst;
    public Image Tiredness;
    public Image ReproductiveUrge;
    public Image Danger;
    
    public override void InitialisePage(EntityInspector _Inspector)
    {
        base.InitialisePage(_Inspector);
        if(Agent.selectedAgent == null)
        {
            ClearGraphs();
            ClearActionImage();
        }
    }
    public override void UpdatePage()
    {
        if(Agent.selectedAgent == null)
        {
            ClearGraphs();
            ClearActionImage();
            return;
        }
        UpdateActionImage();
        UpdateGraphs();
    }

    private void UpdateActionImage()
    {
        string action = Agent.selectedAgent.GetAction();
        switch(action)
        {
            case "GetFood":
                actionImage.sprite = Eat;
                text.text = "Getting Food";
                break;
            case "GetWater":
                actionImage.sprite = Drink;
                text.text = "Getting Water";
                break;
            case "Wander":
                actionImage.sprite = Wander;
                text.text = "Wandering";
                break;
            case "Mating":
                actionImage.sprite = Mating;
                text.text = "Mating";
                break;      
            case "Reproduce":
                actionImage.sprite = Reproduce;
                text.text = "Giving birth";
                break;
            case "Sleep":
                actionImage.sprite = Sleep;
                text.text = "Sleeping";
                break;
            case "Flee":
                actionImage.sprite = Flee;
                text.text = "Fleeing";
                break;
            case "movingToMate":
                actionImage.sprite = movingToMate;
                text.text = "Moving to mate";
                break;
            case "CallingForMate":
                actionImage.sprite = callingMates;
                text.text = "Mating Call";
                break;
            case "Hunt":
                actionImage.sprite = Hunt;
                text.text = "Hunting prey";
                break;
            default:
                actionImage.sprite = NULL;
                text.text = "Null";
                break;
        }
    }
    private void ClearActionImage()
    {
        actionImage.sprite = clear;
        text.text = "";
    }
    private void UpdateGraphs()
    {
        Hunger.fillAmount           = Agent.selectedAgent.GetHunger();
        Thirst.fillAmount           = Agent.selectedAgent.GetThirst();
        Tiredness.fillAmount        = Agent.selectedAgent.GetTiredness();
        ReproductiveUrge.fillAmount = Agent.selectedAgent.GetReproductiveUrge();
        Danger.fillAmount           = Agent.selectedAgent.GetDanger();
    }
    private void ClearGraphs()
    {
        Hunger.fillAmount           = 0;
        Thirst.fillAmount           = 0;
        Tiredness.fillAmount        = 0;
        ReproductiveUrge.fillAmount = 0;
        Danger.fillAmount           = 0;
    }
}
