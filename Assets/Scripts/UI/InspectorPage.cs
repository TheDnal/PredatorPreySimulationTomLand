using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorPage : MonoBehaviour
{
    protected EntityInspector inspector;    
    void Awake()
    {
        gameObject.SetActive(false);
    }
    public virtual void InitialisePage()
    {
        gameObject.SetActive(true);
    } 
    public virtual void UpdatePage(){}
    public virtual void ClosePage()
    {
        gameObject.SetActive(false);
    }
}
