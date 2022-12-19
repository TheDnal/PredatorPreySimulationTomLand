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
    private InspectorPage currentPage;
    private bool Initialised = false;
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
        Buttons.SetActive(false);
    }
    public void Initialise()
    {
        Initialised = true;
        Buttons.SetActive(true);
    }
    void Update()
    {

        if(currentPage != null)
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
    public void ClearSelectedAgent()
    {
        Agent.selectedAgent = null;
    }
}
