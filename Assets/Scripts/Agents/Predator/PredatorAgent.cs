using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorAgent : MonoBehaviour, Agent
{
    /*
  ▄███████▄    ▄████████    ▄████████ ████████▄     ▄████████     ███      ▄██████▄     ▄████████ 
  ███    ███   ███    ███   ███    ███ ███   ▀███   ███    ███ ▀█████████▄ ███    ███   ███    ███ 
  ███    ███   ███    ███   ███    █▀  ███    ███   ███    ███    ▀███▀▀██ ███    ███   ███    ███ 
  ███    ███  ▄███▄▄▄▄██▀  ▄███▄▄▄     ███    ███   ███    ███     ███   ▀ ███    ███  ▄███▄▄▄▄██▀ 
▀█████████▀  ▀▀███▀▀▀▀▀   ▀▀███▀▀▀     ███    ███ ▀███████████     ███     ███    ███ ▀▀███▀▀▀▀▀   
  ███        ▀███████████   ███    █▄  ███    ███   ███    ███     ███     ███    ███ ▀███████████ 
  ███          ███    ███   ███    ███ ███   ▄███   ███    ███     ███     ███    ███   ███    ███ 
 ▄████▀        ███    ███   ██████████ ████████▀    ███    █▀     ▄████▀    ▀██████▀    ███    ███ 
               ███    ███                                                               ███    ███ 
  Description :
  This class controls the predator agent. The predator implements a Goal oriented action planning 
  strategy to survive the simulation. Much like the prey, it must manage a set of discontent values
  which represent its goals. These discontent values (and the goals they represent) are :
  - Hunger (being full-bellied)
  - Thirst (being hydrated)
  - Tiredness (being well-rested)
  - Reproductive urge (successfully having offspring)
  - Danger (Staying safe)
  
  Like the prey, it utilises actions that help it perfrom the right behaviors to satiate these discontent values. 
  What seperates the predator from the prey (besides more complex actions), is an "intelligence" value. This 
  value represents the depth of the predators action-planner. This value represents how many actions the predator 
  can have in its plan. E.g. intelligence = 1  => one action at a time, intelligence = 2  => action plan of 2 actions.
  Predators start at an intelligence of 2, and can evolve higher intelligence. 
  */
    #region Variables
    //References
    private SVision sensorySystem;
    private PathfindingSystem pathfindingSystem;
    private Rigidbody rb;
    
    //Variables
    private string currentActionName = "null";
    private List<AdvancedAction> actions = new List<AdvancedAction>();
    private List<AdvancedAction> actionPlan = new List<AdvancedAction>();
    private AdvancedAction currentAction;
    private bool performingAction = false;
    private Vector3 velocity = Vector3.zero;

    private float age = 0;
    private int gender;
    private Genome genome;
    private int offspringCount = 0;
    
    private Vector2Int currPartition;

    //Discontent Vals
    private float hunger, 
                  thirst, 
                  tiredness, 
                  reproductiveUrge, 
                  danger,
                  pregnancy;
    private float hungerIncrease = 0.0125f, hungerDecrease = -1f,
                  thirstIncrease = 0.02f, thirstDecrease = -1f,
                  tirednessIncrease = 0.0125f, tirednessDecrease = -0.25f,
                  reprodcutiveIncrease = 0.025f, pregnancyIncrease = 0.15f;
    private bool isEating = false,
                isDrinking = false, 
                isSleeping = false,
                isPregnant = false;
    #endregion
    #region Methods
    public void Initialise(int _gender, Genome _genome)
    {
        gender = _gender;
        genome = _genome;
        rb = this.GetComponent<Rigidbody>();
        sensorySystem = this.GetComponent<SVision>();
        pathfindingSystem = this.GetComponent<PathfindingSystem>();
        pathfindingSystem.Initialise(this.gameObject);
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.predator);

        //Set Advanced actions
        AdvWander wander = this.gameObject.AddComponent<AdvWander>();
        wander.Initialise(this);
        actions.Add(wander);

        AdvFindWater water = this.gameObject.AddComponent<AdvFindWater>();
        water.Initialise(this);
        actions.Add(water);

        AdvSleep sleep = this.gameObject.AddComponent<AdvSleep>();
        sleep.Initialise(this);
        actions.Add(sleep);

        AdvHuntPrey hunt = this.gameObject.AddComponent<AdvHuntPrey>();
        hunt.Initialise(this);
        actions.Add(hunt);

    }
    private void Update()
    {
      //Check if out of bounds
      if(transform.position.y < 0){KillAgent("out of bounds");}
      //Perform updates
      UpdateDiscontents();
      UpdateMisc();
      if(performingAction)
      {
        currentAction.UpdateAction();
        return;
      }
      PredatorDiscontentSnapshot snapshot = new PredatorDiscontentSnapshot(hunger,thirst,tiredness,reprodcutiveIncrease,danger,pregnancy);
      currentAction = CalculateBestAction(snapshot);
      currentActionName = currentAction.GetActionName();
      currentAction.PerformAction();
      performingAction = true;
    }
    private void UpdateDiscontents()
    {
        float isSleepingModifier = isSleeping ? 0.25f : 1;

        hunger    = IterateDiscontentValue(!isEating, hunger, hungerIncrease, hungerDecrease, isSleepingModifier);
        if(hunger >= 1){KillAgent("starvation");}
        thirst    = IterateDiscontentValue(!isDrinking, thirst, thirstIncrease, thirstDecrease, isSleepingModifier);
        if(thirst >= 1){KillAgent("Dehydration");}

        tiredness = IterateDiscontentValue(!isSleeping,tiredness, tirednessIncrease, tirednessDecrease);
        if(tiredness >= 1){KillAgent("Exhaustion");}

        if(age < 18)
        {
            reproductiveUrge = 0;
            pregnancy = 0;
            return;
        }

        //Adult only discontent values
        if(gender == 1 && isPregnant){pregnancy = IterateDiscontentValue(true, pregnancy, pregnancyIncrease, 5);}
        else{pregnancy = 0;}

        if(!isPregnant){reproductiveUrge = IterateDiscontentValue(true, reproductiveUrge, reprodcutiveIncrease, 1);}
        else{reproductiveUrge = 0;}
    }
    private float IterateDiscontentValue(bool increasing, float _discontent, float increaseRate, float decreaseRate, float modifier = 1)
    {
        float value;
        if(increasing)
        { 
            value = _discontent + increaseRate * Time.deltaTime * modifier;
        }
        else
        {
            value = _discontent + decreaseRate * Time.deltaTime * modifier;
        }
        if (value > 1){value = 1;}
        else if(value < 0){value = 0;}
        return value;
    }
    private void UpdateMisc()
    {
      //Update partitioning
      Vector2Int oldPartition = currPartition;
      currPartition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
      if(oldPartition != currPartition)
      {
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, oldPartition, PartitionSystem.ObjectType.predator);
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject,PartitionSystem.ObjectType.predator);
      }
      
      //Update velocity
      rb.velocity = velocity;
      transform.LookAt(transform.position + velocity);

      //Update age
      age += Time.deltaTime;


    }
    private AdvancedAction CalculateBestAction(PredatorDiscontentSnapshot snapshot)
    {
      float highscore = 0;
      AdvancedAction bestAction = actions[0];
      foreach(AdvancedAction action in actions)
      {
        if(action.isActionPossible(snapshot))
        {
          float score = action.ActionScore(snapshot);
          if(score > highscore)
          {
            bestAction = action;
            highscore = score;
          }
        }
      }
      return bestAction;
    }
    private void CalculateActionPlan()
    {

    }
    public void Kill()
    {
      if(Agent.selectedAgent.GetGameObject() == this.gameObject){Agent.selectedAgent = null;}
      KillAgent("");
    }
    private void KillAgent(string killMessage)
    {
      if(killMessage != null){Debug.Log("Agent died from " + killMessage);}
      PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, currPartition, PartitionSystem.ObjectType.predator);
      Destroy(this.gameObject);
    }
    private void OnMouseDown()
    {
        Agent.selectedAgent = this;
    }
    #endregion
    #region Getters
    public Vector3 GetVelocity(){return velocity;}
    public Vector2Int GetCurrentPartition(){return currPartition;}
    public PathfindingSystem GetPathfindingSystem(){return pathfindingSystem;}
    public void SetEating(bool _isEating){isEating = _isEating;}
    public void SetDrinking(bool _isDrinking){isDrinking = _isDrinking;}
    public void SetSleeping(bool _isSleeping){isSleeping = _isSleeping;}
    public void ResetReproductiveUrge(){reproductiveUrge = 0;}
    #endregion
    #region Setters
    public void SetVelocity(Vector3 _velocity){velocity = _velocity;}
    public void SetPerformingAction(bool _performingAction){performingAction = _performingAction;}
    #endregion
    //Fulfills the "Agent" interface, which allows inspector classes to monitor this agent.
    #region Interface Methods
    public string GetAction(){return currentActionName;}
    public Agent.AgentType GetAgentType(){return Agent.AgentType.PREDATOR;}
    public Genome GetGenome(){return GeneticsSystem.GetStartingPreyGenome();}
    public SVision GetSensorySystem(){return sensorySystem;}
    public float GetHunger(){return hunger;}
    public float GetThirst(){return thirst;}
    public float GetTiredness(){return tiredness;}
    public float GetReproductiveUrge(){return reproductiveUrge;}
    public float GetDanger(){return danger;}
    public int GetAge(){return Mathf.RoundToInt(age);}
    public int GetOffspringCount(){return offspringCount;}
    public int GetGender(){return gender;}
    public GameObject GetGameObject(){return this.gameObject;}
    #endregion

}
public struct PredatorDiscontentSnapshot
{
    private float hunger, thirst, tiredness, reproductiveUrge, danger, pregnancy;
    public PredatorDiscontentSnapshot(float _hunger, float _thirst, float _tiredness, float _reproductiveUrge, float _danger, float _pregnancy)
    {
      hunger = _hunger;
      thirst = _thirst;
      tiredness = _tiredness;
      reproductiveUrge = _reproductiveUrge;
      danger = _danger;
      pregnancy = _pregnancy;
    }
    public float GetHunger(){return hunger;}
    public float GetThirst(){return thirst;}
    public float GetTiredness(){return tiredness;}
    public float GetReproductiveUrge(){return reproductiveUrge;}
    public float GetDanger(){return danger;}
    public float GetPregnancy(){return pregnancy;}
}