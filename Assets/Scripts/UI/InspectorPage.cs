using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorPage : MonoBehaviour
{
    protected EntityInspector inspector;    
    protected GOPAgent currentAgent;
    void Awake()
    {
        gameObject.SetActive(false);
    }
    public virtual void InitialisePage(EntityInspector _Inspector)
    {
        gameObject.SetActive(true);
        inspector = _Inspector; 
        if(_Inspector.currentEntity == null)
        {
            return;
        }
        currentAgent = _Inspector.currentEntity.GetComponent<GOPAgent>();
    } 
    public virtual void UpdatePage(){}
}
