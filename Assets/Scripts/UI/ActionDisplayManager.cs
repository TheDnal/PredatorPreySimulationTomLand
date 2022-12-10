using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionDisplayManager : MonoBehaviour
{
    public Sprite Eat,Drink,Wander,Reproduce,Sleep , NULL;
    public TMPro.TextMeshProUGUI text;
    public enum Actions{EAT,DRINK,WANDER,REPRODUCE, SLEEP, FINDMATE ,NULL};
    public void SetSprite(Actions action)
    {
        Image image = this.GetComponent<Image>();
        switch(action)
        {
            case Actions.EAT:
                image.sprite = Eat;
                text.text = "Finding Food";
                break;
            case Actions.DRINK:
                image.sprite = Drink;
                text.text = "Finding Water";
                break;
            case Actions.WANDER:
                image.sprite = Wander;
                text.text = "Wandering";
                break;
            case Actions.REPRODUCE:
                image.sprite = Reproduce;
                text.text = "Reproducing";
                break;      
            case Actions.SLEEP:
                image.sprite = Sleep;
                text.text = "Sleeping";
                break;

            default:
                image.sprite = NULL;
                text.text = "Null";
                break;
        }
    }
}
