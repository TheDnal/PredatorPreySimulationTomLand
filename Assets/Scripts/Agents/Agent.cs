using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Agent
{
    
    /*
        Interface for the inspector system. This allows it to read information about the agent,
        without having full access to the agent itself.
        This ensures that :
        A) The inspector can get info regardless of ai solution (fsm, behaviour tree, utility, goap etc)
        B) Promotes abstraction, which makes code more secure and easier to follow
    */
    #region Fields
    public enum AgentType{PREY,PREDATOR};
    public static Agent selectedAgent;
    #endregion
    #region Getters
    ///<summary> Gets a string of the current agent action </summary>
    public string GetAction();
    public AgentType GetAgentType();
    public Genome GetGenome();
    public SVision GetSensorySystem();
    public float GetHunger();
    public float GetThirst();
    public float GetTiredness();
    public float GetReproductiveUrge();
    public float GetDanger();
    public int GetAge();
    public int GetOffspringCount();
    ///<summary> 0 = male, 1 = female. </summary>
    public int GetGender();
    public GameObject GetGameObject();
    public void Kill();
    #endregion
}
