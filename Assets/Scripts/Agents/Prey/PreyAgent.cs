using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyAgent : MonoBehaviour, Agent
{
    #region Main Fields
    [SerializeField] private SVision sensorySystem;
    [SerializeField] private PathfindingSystem pathfindingSystem;
    private PartitionSystem pSystem;
    private List<Agent> potentialMates = new List<Agent>();
    private Vector2Int currPartition;
    private string currentActionName = "Null";
    private Genome genome;
    private int gender;
    private int offspringCount;
    private List<Action> actions = new List<Action>();
    private Action bestAction;
    private Action panicAction;
    private bool panicked;
    private bool initialised = false;
    private bool performingAction = false;
    private Rigidbody rb;
    private Vector3 velocity = Vector3.zero;
    private int actualAge = 0;
    private float age = 0;
    //Variables that help the prey decide if an action is urgent enough to override the current action
    #endregion
    #region Discontent fields
    private float hunger, 
                  thirst, 
                  tiredness, 
                  reproductiveUrge, 
                  danger,
                  pregnancy;
    private float hungerIncrease = 0.025f, hungerDecrease = -0.5f,
                 thirstIncrease = 0.033f, thirstDecrease = -1f,
                 tirednessIncrease = 0.0125f, tirednessDecrease = -0.25f,
                 reprodcutiveIncrease = 0.025f, pregnancyIncrease = 0.33f;
    private bool isEating = false,
                isDrinking = false, 
                isSleeping = false,
                isPregnant = false;
    #endregion
    #region Methods
    public void Initialise(int _gender, Genome _genome)
    {   
        //Store intialisation variables
        gender = _gender;
        genome = _genome;
        hungerIncrease *= genome.respirationRate;
        rb = this.GetComponent<Rigidbody>();
        sensorySystem = this.GetComponent<SVision>();
        sensorySystem.Configure(genome.visionRadius,genome.smellRadius,genome.visionAngle);
        pSystem = PartitionSystem.instance;
        pathfindingSystem.Initialise(this.gameObject);
        UpdatePartitioning();
        initialised = true;

        if(gender == 0)
        {
            this.GetComponent<MeshRenderer>().material = EntitySpawner.instance.MalePreyMat;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = EntitySpawner.instance.FemalePreyMat;
        }
        //Set up all actions
        WanderAction wander = this.gameObject.AddComponent<WanderAction>();
        actions.Add(wander);
        GetFoodAction getFood = this.gameObject.AddComponent<GetFoodAction>();
        actions.Add(getFood);
        GetWaterAction getWater = this.gameObject.AddComponent<GetWaterAction>();
        actions.Add(getWater);

        FleeAction flee = this.gameObject.AddComponent<FleeAction>();
        actions.Add(flee);
        panicAction = flee;

        SleepAction sleep = this.gameObject.AddComponent<SleepAction>();
        actions.Add(sleep);
        MatingCall callMate = this.gameObject.AddComponent<MatingCall>();
        actions.Add(callMate);
        RespondToCall respond = this.gameObject.AddComponent<RespondToCall>();
        actions.Add(respond);
        if(gender == 1)
        {
            GiveBirth birthing = this.gameObject.AddComponent<GiveBirth>();
            actions.Add(birthing);
        }
    }
    public void Update()
    {
        //Return if not initialised
        if(!initialised)
        {
            return;
        }
        if(transform.position.y < 0){Debug.Log("agent fell out of bounds"); killAgent();}
        //Update values
        UpdateDiscontent();
        UpdatePartitioning();
        UpdateAge();
        UpdateMisc();
        //Get danger value
        danger = sensorySystem.GetSensedDanger();
        //If danger is above a certain threshhold, then override current action and panic
        if(danger > 0)
        {
            if(bestAction != null && bestAction.CanActionBeOverriden())
            {
                bestAction.ExitAction();
                bestAction = panicAction;
                currentActionName = bestAction.actionName;
                bestAction.PerformAction();
                performingAction = true;
            }
        }
        if(performingAction)
        {
            bestAction.UpdateAction();
            return;
        }
        //else find a new action
        bestAction = GetBestAction(actions);
        currentActionName = bestAction.actionName;
        bestAction.PerformAction();
        performingAction = true;
    }
    ///<summary> Updates the discontent values of this agent </summary>
    private void UpdateDiscontent()
    {
        float isSleepingModifier = isSleeping ? 0.25f : 1;

        hunger    = UpdateDiscontentValue(!isEating, hunger, hungerIncrease, hungerDecrease, isSleepingModifier);
        if(hunger >= 1){killAgent("starvation");}
        thirst    = UpdateDiscontentValue(!isDrinking, thirst, thirstIncrease, thirstDecrease, isSleepingModifier);
        if(thirst >= 1){killAgent("Dehydration");}

        tiredness = UpdateDiscontentValue(!isSleeping,tiredness, tirednessIncrease, tirednessDecrease);
        if(tiredness >= 1){killAgent("Exhaustion");}

        if(actualAge < 18)
        {
            reproductiveUrge = 0;
            pregnancy = 0;
            return;
        }

        //Adult only discontent values
        if(gender == 1 && isPregnant){pregnancy = UpdateDiscontentValue(true, pregnancy, pregnancyIncrease, 5);}
        else{pregnancy = 0;}

        if(!isPregnant){reproductiveUrge = UpdateDiscontentValue(true, reproductiveUrge, reprodcutiveIncrease, 1);}
        else{reproductiveUrge = 0;}
    }

    ///<summary> Updates a specific discontent value between 0 and 1. If you dont want to set a modifier, just leave it at 1 </summary>
    private float UpdateDiscontentValue(bool increasing,float _discontent, float increaseRate, float decreaseRate, float modifier = 1)
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
    ///<summary> Updates the partition data of this agent </summary>
    private void UpdatePartitioning()
    {
        Vector2Int oldPartition = currPartition;
        currPartition = pSystem.WorldToPartitionCoords(transform.position);
        if(oldPartition != currPartition)
        {
            pSystem.RemoveGameObjectFromPartition(this.gameObject,oldPartition, PartitionSystem.ObjectType.prey);
            pSystem.AddGameObjectToPartition(this.gameObject,PartitionSystem.ObjectType.prey);
        }
    }
    ///<summary> Updates any misc values </summary>
    private void UpdateMisc()
    {
        rb.velocity = velocity;
        transform.LookAt(transform.position + velocity);
    }
    ///<summary> Updates the age of the agent </summary>
    private void UpdateAge()
    {
        age += Time.deltaTime;
        actualAge = Mathf.RoundToInt(age);
        //Set agent size
        if(actualAge < 5)
        {
            transform.localScale = new Vector3(0.2f,0.4f,0.2f) * 5/18;
        }
        else if(actualAge < 18)
        {
            transform.localScale = new Vector3(0.2f,0.4f,0.2f) * (age * genome.size /18);
        }
        //else if(age > 80){killAgent();}
        else
        {
            transform.localScale = new Vector3(0.2f,0.4f,0.2f) * genome.size;
        }
    }
    private Action GetBestAction(List<Action> _actions)
    {
        float highestScore = 0;
        Action bestAction = _actions[0];
        foreach(Action action in actions)
        {
            if(action.isActionPossible(this))
            {
                float score = action.ActionScore();
                if(score > highestScore)
                {
                    bestAction = action;
                    highestScore = score;
                }
            }
        }
        return bestAction;
    }
    private void killAgent(string causeOfDeath = "")
    {
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, currPartition, PartitionSystem.ObjectType.prey);
        EntitySpawner.instance.currentPopulation--;
        Destroy(this.gameObject);
    }
    public void Kill()
    {
        if(Agent.selectedAgent == this)
        {
            Agent.selectedAgent = null;
        }
        killAgent("");
    }
    private void OnMouseDown()
    {
        if(Agent.selectedAgent == this)
        {
            Agent.selectedAgent = null;
            Debug.Log("agent is null");
            return;
        }
        Agent.selectedAgent = this;
    }
    #endregion
    #region Getters
    public PathfindingSystem GetPathfindingSystem(){return pathfindingSystem;}
    public float GetPregnancy(){return pregnancy;}
    
    #endregion
    #region Setters
    public void IncrementOffspringCount(){offspringCount ++;}
    public void SetEating(bool _isEating){isEating = _isEating;}
    public void SetDrinking(bool _isDrinking){isDrinking = _isDrinking;}
    public void SetSleeping(bool _isSleeping){isSleeping = _isSleeping;}
    public void ResetReproduction(){reproductiveUrge = 0;}
    public void ResetPregnancy(){pregnancy = 0; isPregnant = false;}
    public void Inpregnate(Genome fatherGenome){isPregnant = true;}
    public void SetVelocity(Vector3 _velocity){velocity = _velocity * genome.speed * 1.5f;}
    public void SetPerformingAction(bool _performingAction){performingAction = _performingAction;}
    public bool TryBecomeMate(Agent mate)
    {
        if (isPregnant) { return false; }
        else if (potentialMates.Count < 2)
        {
            potentialMates.Add(mate);
            return true;
        }
        return false;
    }
    public void TryMate(Genome _genome)
    {
        if (gender != 1) { return; }
        if (!isPregnant)
        {
            isPregnant = true;
            reproductiveUrge = 0;
            potentialMates.Clear();
        }
    }
    #endregion
    #region Interface Methods
    public string GetAction(){return currentActionName;}
    public Agent.AgentType GetAgentType(){return Agent.AgentType.PREY;}
    public Genome GetGenome(){return genome;}
    public float GetHunger(){return hunger;}
    public float GetThirst(){return thirst;}
    public float GetTiredness(){return tiredness;}
    public float GetReproductiveUrge(){return reproductiveUrge;}
    public float GetDanger(){return danger;}
    public int GetAge(){return actualAge;}
    public int GetGender(){return gender;}
    public int GetOffspringCount(){return offspringCount;}
    public SVision GetSensorySystem(){return sensorySystem;}
    public Vector2Int GetCurrentPartition(){return currPartition;}
    public GameObject GetGameObject(){return this.gameObject;}
    public void SetAge(int _age){age =_age;}
    #endregion
}