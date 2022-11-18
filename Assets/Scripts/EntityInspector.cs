using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    public TextMeshProUGUI CurrentEntityAction;
    [Space(3)]
    public TextMeshProUGUI Age;
    public TextMeshProUGUI hunger;
    public TextMeshProUGUI thirst;
    public TextMeshProUGUI ReproductiveUrge;
    public TextMeshProUGUI Sleepyness;
    [Space(6), Header("UI Settings")]
    public float transitionSpeed = 0;
    public float hiddenXPos, visibleXPos;
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
        targetXPos = hiddenXPos;
    }
    public void SetSelectedEntity(GameObject entity, EntityType _entityType)
    {
        entityType = _entityType;
        currentEntity = entity;
        EntityName.text = currentEntity.name;
        currSelectedPrey = entity.GetComponent<PreyAgent>();
        UpdateEntityData();
    }
    void Update()
    {
        if(currentEntity != null)
        {   
            UpdateEntityData();
        }
        RectTransform rect = UIElement.GetComponent<RectTransform>();
        Vector3 pos = rect.position;
        if(rect.position.x != targetXPos)
        {
            pos.x = targetXPos;
            rect.position = pos;
        }
    }
    private void UpdateEntityData()
    {
        if(currentEntity == null)
        {
            return;
        }
        Age.text          = "Age :                  \n" + currSelectedPrey.age.ToString();

        float _hunger,_thirst,_sleepyness,_reproductiveUrge;

        _hunger           = currSelectedPrey.GetHunger();
        _thirst           = currSelectedPrey.GetThirst();
        _sleepyness       = currSelectedPrey.GetTiredness();
        _reproductiveUrge = currSelectedPrey.GetReproduction(); 

        hunger.text           = "Hunger :           \n" + ConvertToPercentage(_hunger).ToString();
        thirst.text           = "Thirst :           \n" + ConvertToPercentage(_thirst).ToString(); 
        Sleepyness.text       = "Sleepyness :       \n" + ConvertToPercentage(_sleepyness).ToString();
        ReproductiveUrge.text = "Reproductive Urge :\n" + ConvertToPercentage(_reproductiveUrge).ToString();

    }
    private int ConvertToPercentage(float _float)
    {
        _float *= 100;
        int _int = Mathf.RoundToInt(_float);
        return _int;
    }
    public void ToggleUIDisplay()
    {
        UIEnabled = !UIEnabled;
        targetXPos = UIEnabled ? visibleXPos : hiddenXPos;
    }
}
