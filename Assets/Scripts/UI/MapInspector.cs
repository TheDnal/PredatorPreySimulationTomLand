using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MapInspector : MonoBehaviour
{
    /*
        This script controls all the UI elements that show the user map info.
        It functions as a simple statemachine
    */
    public static MapInspector instance;
    private InspectorPage currentPage;
    public GameObject UIElements;
    public float closedXPos, openXPos;
    bool open = false, initalisated = false;
    public void Awake()
    {
        //Singleton initalisation
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        UIElements.SetActive(false);
    }
    public void Initialise()
    {
        initalisated = true;
        UIElements.SetActive(true);
    }
    public void Update()
    {
        if(!initalisated){return;}
        float xPos;
        if(open)
        {
            xPos = openXPos;
        }
        else
        {
            xPos = closedXPos;
        }
        Vector3 pos = UIElements.GetComponent<RectTransform>().anchoredPosition;
        pos.x = xPos;
        UIElements.GetComponent<RectTransform>().anchoredPosition = pos; 

        if(currentPage != null)
        {
            currentPage.UpdatePage();
        }
    }
    public void SetPage(InspectorPage page)
    {
        if(currentPage == page)
        {
            currentPage.ClosePage();
            currentPage = null;
            open = false;
            Debug.Log(open);
            return;
        }
        else if(currentPage != null)
        {
            currentPage.ClosePage();
        }
        open = true;
        currentPage = page;
        Debug.Log(open);
        page.InitialisePage();
    }
}
