using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPAgent : MonoBehaviour
{
    /*
        Class for the Goal-oriented-planner AI. This agent
        utilised Goals and Actions 
        (see the Goal class and Action class). 
        Goals are the overall objectives of the AI, such 
        as staying well fed, staying hydrated, well slept, 
        safe etc. This is represented by a value that the AI
        will try to keep down by executing actions.

        Actions are behaviours that allow the AI to fulfill 
        its goals. Actions have scores, with the action having
        highest score being executed by the AI. The score of
        an action is defined by how well it fulfills goals and 
        how much it hinders other goals. 
    */

    ////Goals
    //These values represent the AI's "urge" to deal with the
    //goal. 
    protected float tiredness = 0;
    protected float hunger = 0;
    protected float thirst = 0;
    protected float reproduction = 0;
    protected float danger = 0;
    ////

    protected Vector2Int currPartition;
    //Goal discontent speeds
    
    protected float tirednessIncrease = 0.04f;
    protected float hungerIncrease = 0.05f;
    protected float hungerDecrease = 1f;
    protected float reproductionIncrease = 0.035f;
    //
    //Which AI is being highlighted by the user
    public static GOPAgent selectedAgent;

    //public UIManager uiManager;

    //Speed modifier
    protected float speedModifier = 1;
    //Cached rb
    protected Rigidbody rb;

    //Locks the AI out from selecting new actions.
    protected bool performingAction = false;
    //Prevents hunger loss (i.e. while eating)
    protected bool isEating = false;
    protected bool isDrinking = false;

    ////Action arrays
    protected List<Action> actions = new List<Action>();
    ////

    protected Vector3 velocity;

    #region Virtual Methods
    public virtual void Initialise()
    {
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine(i_GenerateDiscontent());
    }
    protected IEnumerator i_GenerateDiscontent()
    {
        while(true)
        {
            if(tiredness <= 1){ tiredness += tirednessIncrease * Time.deltaTime;}
            else{tiredness = 1;}
            if(!isEating)
            {
                if(hunger <= 1){hunger += hungerIncrease * Time.deltaTime;}
                else{hunger = 1; killAgent();}
            }
            else
            {
                if(hunger >= 0){hunger -= hungerDecrease * Time.deltaTime;}
                else{hunger = 0;}
            }
            if(reproduction <= 1){reproduction += reproductionIncrease * Time.deltaTime;}
            else{reproduction = 1;}

            if(danger <= 1){/*TODO*/}
            else{danger =1;}
            yield return new WaitForEndOfFrame();
        }
    }

    //Method that returns the best possible action
    protected Action CalculateBestAction()
    {
        float bestScore = float.MinValue;
        Action bestAction = actions[0];
        foreach(Action _action in actions)
        {
            if(_action.isActionPossible(this))
            {
                float actionScore = _action.ActionScore();
                if(bestScore < actionScore)
                {
                    bestScore = actionScore;
                    bestAction = _action;
                }
            }
        }
        return bestAction;
    }

    #endregion

    #region Getters
    public float GetTiredness(){return tiredness;}
    public float GetHunger(){return hunger;}
    public float GetReproduction(){return reproduction;}
    public float GetDanger(){return danger;}
    public float GetSpeedModifier(){return speedModifier;}
    public Vector2Int getCurrPartition(){return currPartition;}
    #endregion

    #region Setters
    public void SetEating(bool _isEating){isEating = _isEating;}
    public void SetPerformingAction(bool _isPerformingAction){performingAction = _isPerformingAction;}
    private void SetState()
    {
        //TODO
    }
    public void setVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void killAgent()
    {
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, getCurrPartition(), PartitionSystem.ObjectType.agent);
        Debug.Log("Agent " + this.gameObject.name + " died");
        Destroy(this.gameObject);
    }
    #endregion
}
