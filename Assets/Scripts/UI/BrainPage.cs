using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BrainPage : InspectorPage
{
    [Header("Action images")]
    public Image actionImage;
    public Sprite Eat,Drink,Wander,Mating,Reproduce,Sleep, Flee, NULL;
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
    }
    public override void UpdatePage()
    {
        if(currentAgent == null)
        {
            return;
        }
        UpdateActionImage();
        UpdateGraphs();
    }

    private void UpdateActionImage()
    {
        string action = currentAgent.GetCurrentAction();
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
            default:
                actionImage.sprite = NULL;
                text.text = "Null";
                break;
        }
    }
    private void UpdateGraphs()
    {
        Hunger.fillAmount           = currentAgent.GetHunger();
        Thirst.fillAmount           = currentAgent.GetThirst();
        Tiredness.fillAmount        = currentAgent.GetTiredness();
        ReproductiveUrge.fillAmount = currentAgent.GetReproduction();
        Danger.fillAmount           = currentAgent.GetDanger();
    }
}
