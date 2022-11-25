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
    public GameObject Buttons;
    [Space(3)]
    public GameObject Hunger,Thirst,Tiredness,ReproductiveUrge;
    [Space(6), Header("UI Settings")]
    public float transitionSpeed = 0;
    public float openButtonPos,closedButtonPos;
    private float targetXPos;
    public GameObject currentEntity;
    private PreyAgent currSelectedPrey;
    private InspectorPage currentPage;
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
        Buttons.SetActive(true);
    }
    public void SetSelectedEntity(GameObject entity, EntityType _entityType)
    {
        entityType = _entityType;
        currentEntity = entity;
        EntityName.text = currentEntity.name;
        currSelectedPrey = entity.GetComponent<PreyAgent>();
        if(currentPage != null)
        {
            currentPage.InitialisePage(this);
        }
    }
    void Update()
    {
        if(currentEntity != null && currentPage != null)
        {   
            currentPage.UpdatePage();
        }
        if(currentPage != null)
        {
            targetXPos = openButtonPos;
        }
        else
        {
            targetXPos = closedButtonPos;
        }
        Vector3 pos = Buttons.GetComponent<RectTransform>().position;
        pos.x = targetXPos;
        Buttons.GetComponent<RectTransform>().position = pos;
    }
    public void SetCurrentPage(InspectorPage page)
    {
        if(currentPage == page)
        {
            currentPage.ClosePage();
            currentPage.gameObject.SetActive(false);
            currentPage = null;
            return;
        }
        if(currentPage != null)
        {
            currentPage.ClosePage();
            currentPage.gameObject.SetActive(false);
        }
        currentPage = page;
        page.gameObject.SetActive(true);
        page.InitialisePage(this);
    }
}