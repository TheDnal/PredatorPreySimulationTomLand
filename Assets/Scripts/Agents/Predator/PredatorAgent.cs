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
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject, PartitionSystem.ObjectType.agent);

        //Set Advanced actions
        AdvWander wander = this.gameObject.AddComponent<AdvWander>();
        wander.Initialise(this);
        actions.Add(wander);
        Debug.Log(actions.Count);
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
      currentAction = actions[0];
      currentAction.PerformAction();
      performingAction = true;
    }
    private void UpdateDiscontents()
    {

    }
    private void IterateDiscontentValue(bool increasing, float _discontent, float increaseRate, float decreaseRate, float modifier = 1)
    {

    }
    private void UpdateMisc()
    {
      //Update partitioning
      Vector2Int oldPartition = currPartition;
      currPartition = PartitionSystem.instance.WorldToPartitionCoords(transform.position);
      if(oldPartition != currPartition)
      {
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, oldPartition, PartitionSystem.ObjectType.agent);
        PartitionSystem.instance.AddGameObjectToPartition(this.gameObject,PartitionSystem.ObjectType.agent);
      }
      
      //Update velocity
      rb.velocity = velocity;
      transform.LookAt(transform.position + velocity);

      //Update age
      age += Time.deltaTime;


    }
    private void CalculateBestAction()
    {

    }
    private void CalculateActionPlan()
    {

    }
    private void KillAgent(string killMessage)
    {
      if(killMessage != null){Debug.Log("Agent died from " + killMessage);}
      PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, currPartition, PartitionSystem.ObjectType.agent);
      Destroy(this.gameObject);
    }
    private void OnMouseDown()
    {
        Agent.selectedAgent = this;
    }
    #endregion
    #region Getters
    public Vector3 GetVelocity(){return velocity;}
    #endregion
    #region Setters
    public void SetVelocity(Vector3 _velocity){velocity = _velocity;}
    public void SetPerformingAction(bool _performingAction){performingAction = _performingAction;}
    #endregion
    //Fulfills the "Agent" interface, which allows inspector classes to monitor this agent.
    #region Interface Methods
    public string GetAction(){return "";}
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