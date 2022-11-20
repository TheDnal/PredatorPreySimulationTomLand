using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EntityInspector : MonoBehaviour
{
    //Singleton
    public static EntityInspector instance;
    private string entityName;
    private bool UIEnabled = false;
    public enum EntityType{prey,predator};
    public EntityType entityType;


    [Header("UI Elements")]
    public GameObject UIElement;
    public TextMeshProUGUI EntityName;
    public GameObject SummaryUI, BrainUI, SensesUI,GeneticsUI, Buttons;
    int screenNum = 0;
    public ActionDisplayManager actionDisplay;
    [Space(3)]
    public TextMeshProUGUI Age;
    public GameObject Hunger,Thirst,Tiredness,ReproductiveUrge;
    [Space(6), Header("UI Settings")]
    public float transitionSpeed = 0;
    public float openButtonPos,closedButtonPos;
    private float targetXPos;
    private GameObject currentEntity;
    private PreyAgent currSelectedPrey;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        targetXPos = closedButtonPos;
    }
    public void SetSelectedEntity(GameObject entity, EntityType _entityType)
    {
        entityType = _entityType;
        currentEntity = entity;
        EntityName.text = currentEntity.name;
        currSelectedPrey = entity.GetComponent<PreyAgent>();
        UpdateEntityData();
    }
    public void SetCurrentUIScreen(int _screenNum)
    {
        if(screenNum == _screenNum)
        {
            screenNum = 0;
            return;
        }
        else
        {
            screenNum = _screenNum;
        }
    }
    void Update()
    {
        if(currentEntity != null)
        {   
            UpdateEntityData();
        }
        SummaryUI.SetActive(false);
        BrainUI.SetActive(false);
        SensesUI.SetActive(false);
        GeneticsUI.SetActive(false);
        targetXPos = openButtonPos;
        switch(screenNum)
        {
            case 0:
                targetXPos = closedButtonPos;
                break;
            case 1:
                SummaryUI.SetActive(true);
                break;
            case 2:
                BrainUI.SetActive(true);
                break;
            case 3:
                SensesUI.SetActive(true);
                break;
            case 4:
                GeneticsUI.SetActive(true);
                break;
            default:
                targetXPos = closedButtonPos;
                break;
        }
        Vector3 pos = Buttons.GetComponent<RectTransform>().position;
        pos.x = targetXPos;
        Buttons.GetComponent<RectTransform>().position = pos;
    }
    private void UpdateEntityData()
    {
        if(currentEntity == null)
        {
            actionDisplay.SetSprite(ActionDisplayManager.Actions.NULL);
            return;
        }
        Age.text                 = "Age \n" + currSelectedPrey.age.ToString();
        ActionDisplayManager.Actions currentAction;

        switch(currSelectedPrey.GetCurrentAction())
        {
            case "GetFood":
                currentAction = ActionDisplayManager.Actions.EAT;
                break;
            case "GetWater":
                currentAction = ActionDisplayManager.Actions.DRINK;
                break;
            case "Wander":
                currentAction = ActionDisplayManager.Actions.WANDER;
                break;
            case "Reproduce":
                currentAction = ActionDisplayManager.Actions.REPRODUCE;
                break;
            case "Sleep":
                currentAction = ActionDisplayManager.Actions.SLEEP;
                break;
            default :
                currentAction = ActionDisplayManager.Actions.NULL;
                break;
        }
        actionDisplay.SetSprite(currentAction);


        Hunger.GetComponent<Image>().fillAmount           = currSelectedPrey.GetHunger();
        Thirst.GetComponent<Image>().fillAmount           = currSelectedPrey.GetThirst();
        Tiredness.GetComponent<Image>().fillAmount        = currSelectedPrey.GetTiredness();
        ReproductiveUrge.GetComponent<Image>().fillAmount = currSelectedPrey.GetReproduction();
    }
    private int ConvertToPercentage(float _float)
    {
        _float *= 100;
        int _int = Mathf.RoundToInt(_float);
        return _int;
    }
    
}
