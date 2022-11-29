using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPAgent : MonoBehaviour
{
    #region Description
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
    #endregion
    #region Fields

    public bool showGizmos = false;

    public float tiredness = 0;
    public float hunger = 0;
    public float thirst = 0;
    public float reproduction = 0;
    public float danger = 0;
    public float pregnancy = 0;
    public float age = 0;
    public float offspring = 0;
    private float actualAge = 0;
    protected int gender = 0; //0 = male, 1 = female
    public bool validFemale = false;
    public bool suitableMale = false;
    public bool pregnant = false;
    private GameObject maleMate;
    public string agentType;
    protected string currentActionName;
    ////

    protected Vector2Int currPartition;

    //Goal discontent speeds
    protected Genome genome;
    protected float tirednessIncrease = 0.0125f;
    protected float tirednessDecrease = 0.25f;

    protected float pregnancyIncrease = 0.25f;
    protected float hungerIncrease = 0.025f;
    protected float hungerDecrease = 1f;

    protected float thirstIncrease = 0.033f;
    protected float thirstDecrease = 1f; 

    protected float reproductionIncrease = 0.05f;
    protected float reproductionDecrease = 1f;
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
    //Reduces discontent when true, increases when false
    [SerializeField]
    protected bool isEating = false;
    protected bool isDrinking = false;
    protected bool isSleeping = false;
    protected bool isReproducing = false;
    ////Action arrays
    protected List<Action> actions = new List<Action>();
    ////

    ////Dijkstra arrays
    private List<Partition> partitions = new List<Partition>();
    private List<Node> nodeGrid = new List<Node>();
    public SVision sensorySystem;
    protected Vector3 velocity;
    #endregion
    #region Virtual Methods
    public virtual void Initialise()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    protected void UpdateDiscontent()
    {
        float sleepModifier = 1;
        float reproductionModifier = 1;
        if(!isSleeping)
        {
            if(tiredness <= 1){ tiredness += tirednessIncrease * Time.deltaTime;}
            else{tiredness = 1;}
        }
        else
        {
            sleepModifier = 0.25f;
            reproductionModifier = 0;
            if(tiredness >= 0){ tiredness -= tirednessDecrease * Time.deltaTime;}
            else{tiredness = 0;}
        }
        
        if(!isEating)
        {
            if(hunger <= 1){hunger += hungerIncrease * sleepModifier * Time.deltaTime;}
            else{hunger = 1; killAgent();}
        }
        else
        {
            if(hunger >= 0){hunger -= hungerDecrease * Time.deltaTime;}
            else{hunger = 0;}
        }
        
        if(!isDrinking)
        {
            if(thirst <= 1){thirst += thirstIncrease * sleepModifier * Time.deltaTime;}
            else{thirst = 1; killAgent();} 
        }
        else
        {
            if(thirst >= 0){thirst -= thirstDecrease * Time.deltaTime;}
            else{thirst = 0;}
        }
        
        if(pregnant && gender == 1)
        {
            if(pregnancy <= 1){pregnancy += pregnancyIncrease * Time.deltaTime;}
            else{pregnancy = 1;}
        }
        else
        {
            pregnancy = 0;
        }
        //Age
        actualAge += Time.deltaTime;
        age = Mathf.RoundToInt(actualAge);
        if(actualAge < 20)
        {
            if(actualAge <5)
            {
                transform.localScale = new Vector3(0.2f,0.4f,0.2f) * 5 /20;
            }
            else
            {
                transform.localScale = (new Vector3(0.2f,0.4f,0.2f) * actualAge /20) * genome.size;
            }
        }
        else
        {
            transform.localScale = new Vector3(0.2f,0.4f,0.2f) * genome.size;
        }
        if(!isReproducing && age > 10 && !pregnant)
        {
            if(hunger >0.5f || thirst > 0.5f)
            {
                reproductionModifier = 0;
            }
            if(reproduction <= 1){reproduction += reproductionIncrease * reproductionModifier * Time.deltaTime;}
            else{reproduction = 1; validFemale = true;}
        }
        else
        {
            if(reproduction >= 0){reproduction -= reproductionDecrease * Time.deltaTime;}
            else{reproduction = 0;}
        }

        if(danger <= 1){/*TODO*/}
        else{danger =1;}
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
    protected bool requiresPathfinding()
    {
        return false;
    }
    protected Vector3 currPos, destinationPos;
    protected List<Partition> cachedPartitions;
    #endregion
    #region Getters
    public float GetTiredness(){return tiredness;}
    public float GetHunger(){return hunger;}
    public float GetThirst(){return thirst;}
    public float GetReproduction(){return reproduction;}
    public float GetDanger(){return danger;}
    public float GetSpeedModifier(){return speedModifier;}
    public Vector2Int getCurrPartition(){return currPartition;}
    public GameObject getMaleMate()
    {
        return maleMate;
    }
    public string GetCurrentAction()
    {
        if(currentActionName == null)
        {
            return "null";
        }
        return currentActionName;
    }
    public int GetGender()
    {
        return gender;
    }
    public Genome getGenome()
    {
        return genome;
    }
    #endregion
    #region Setters
    public void SetEating(bool _isEating){isEating = _isEating;}
    public void SetDrinking(bool _isDrinking){isDrinking = _isDrinking;}
    public void SetReproduction(bool _isReproducing){isReproducing = _isReproducing;}
    public void ResetReproductiveUrge()
    {
        reproduction = 0;
    }
    public void SetSleeping(bool _isSleeping){isSleeping = _isSleeping;}
    public void SetPerformingAction(bool _isPerformingAction){performingAction = _isPerformingAction;}
    public void setVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void killAgent()
    {
        EntitySpawner.instance.currentPopulation--;
        PartitionSystem.instance.RemoveGameObjectFromPartition(this.gameObject, getCurrPartition(), PartitionSystem.ObjectType.agent);
        EntitySpawner.instance.RemoveEntity(this.gameObject);
        Destroy(this.gameObject);
    }
    public void SetGender(int _gender)
    {
        gender = _gender;
    }
    public void SetMaleMate(GameObject male)
    {
        maleMate = male;
        validFemale = false;
    }
    public void Mate()
    {
        maleMate = null;
        pregnant = true;
        validFemale = false;
    }
    public void SetGenome(Genome _genome)
    {
        genome = _genome;
    }
    #endregion 
    #region Dijkstra Fields
    protected Node[,] nodes;
    Vector2Int gridSize;
    Vector2Int startPos,endPos;
    Node startNode,endNode;
    Vector2Int gridCorner;
    protected List<Vector2Int> partitionPath = new List<Vector2Int>();
    protected List<Vector3> currentPath = new List<Vector3>();
    public bool arrivedAtDestination = false;
    #endregion
    #region DijkstraAlgorithm

    protected List<Vector2Int> GetPathToPartition(Vector2Int endPartition, int pathfindingRadius)
    {
        Vector2Int bounds = PartitionSystem.instance.size;
        if(endPartition.x < 0 || endPartition.x >= bounds.x || endPartition.y < 0|| endPartition.y >= bounds.y)
        {
            return null;
        }
        
        if(!PartitionSystem.instance.partitions[endPartition.x,endPartition.y].IsTraversable())
        {
            return null;
        }
        startPos = currPartition;
        endPos = endPartition;
        GenerateNodeGrid(pathfindingRadius, endPos);
        partitionPath = DijkstraCompute();
        return partitionPath;
    }
    private void GenerateNodeGrid(int radius, Vector2Int end)
    {
        //Length of node grid
        int length = 1 + 2* radius;
        //Generate node grid
        nodes = new Node[length,length];

        gridSize = new Vector2Int(length,length);
        //Get grid corner coord, in world coords
        gridCorner = currPartition - new Vector2Int(radius,radius);
        //Get end coord in relative coords
        Vector2Int endCoord = end - gridCorner;

        Vector2Int bounds = PartitionSystem.instance.size;
        //Generate node grid, starting from the grid corner
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < length; j++)
            {
                Vector2Int pos = new Vector2Int(i,j);
                bool traversable;
                Vector2Int worldPos = pos + gridCorner;
                if(worldPos.x < 0 || worldPos.x >= bounds.x || worldPos.y < 0|| worldPos.y >= bounds.y)
                {
                    traversable = false;
                }
                else
                {
                    traversable = PartitionSystem.instance.partitions[worldPos.x,worldPos.y].IsTraversable();
                }
                Node newNode = new Node(pos,traversable);
                nodes[i,j] = newNode;
            }
        }
        //Get start and end node
        startNode = nodes[radius,radius];
        endNode = nodes[endCoord.x,endCoord.y];
    }
    private List<Vector2Int> DijkstraCompute()
    {
               /*
        Begin at start node
        get adjacent nodes
        get distance from start node to adjacent node. if dist + start node dist < adjacent dist,
        then adjacent dist = dist + start dist
        mark adjacent nodes as visited
        get all their neighbours (no repeats or visited nodes)
        repeat until no nodes left or reach end node visited
        */ 

        partitionPath = new List<Vector2Int>();
        //Get start node
        Node currentNode = startNode;
        currentNode.distance = 0;
        List<Node> nodesToCompute = new List<Node>();
        //Get Adjacent nodes
        nodesToCompute.AddRange(GetAdjacentNodes(currentNode.pos));
        List<Node> nodesToPrune = new List<Node>();
        foreach(Node n in nodesToCompute)
        {
            if(!n.traversable)
            {
                nodesToPrune.Add(n);
            }
        }
        foreach(Node n in nodesToPrune)
        {
            nodesToCompute.Remove(n);
        }
        foreach(Node n in nodesToCompute)
        {
            n.rootNode = currentNode.pos;
        }
        int x =0;
        //Repeat until no nodes left or reach end node visited
        while(nodesToCompute.Count > 0)
        {
            float distance;
            List<Node> nextNodesToCompute = new List<Node>();
            foreach(Node adjacentNode in nodesToCompute)
            {
                x++;
                currentNode = nodes[adjacentNode.rootNode.x, adjacentNode.rootNode.y];
                //Get distance from adjacent node to root node
                distance = Vector2Int.Distance(adjacentNode.pos, currentNode.pos);
                if(distance + currentNode.distance < adjacentNode.distance)
                {
                    //Set new distance
                    adjacentNode.distance = distance + currentNode.distance;
                    //Mark as visited
                    adjacentNode.visited = true;
                    adjacentNode.chainNode = currentNode.pos;
                    List<Node> adjacentNodes = GetAdjacentNodes(adjacentNode.pos);
                    foreach(Node n in adjacentNodes)
                    {
                        if(!n.visited && !nextNodesToCompute.Contains(n) && n.traversable)
                        {
                            n.rootNode = adjacentNode.pos;
                            nextNodesToCompute.Add(n);
                        }
                    }
                }
            }
            nodesToCompute.Clear();
            nodesToCompute.AddRange(nextNodesToCompute);
        }
        if(!endNode.visited){
            return null;
        }
        List<Vector2Int> reversePath = new List<Vector2Int>();
        Vector2Int currStep = endNode.pos;
        x = 0;
        while(currStep != startNode.pos && x < 99)
        {
            x++;
            reversePath.Add(currStep);
            currStep = nodes[currStep.x,currStep.y].chainNode;
        }
        reversePath.Add(startNode.pos);
        partitionPath = new List<Vector2Int>();

        for(int i = reversePath.Count - 1; i >=0 ; i--)
        {
            partitionPath.Add(reversePath[i] - startNode.pos);
        }
        return partitionPath;
    }
    
    public bool isTargetReachable(Vector3 target)
    {
        //Actions will use this to make sure their target location is valid
        Vector2Int targetPartition = PartitionSystem.instance.WorldToPartitionCoords(target);
        List<Vector2Int> dijkstraPath = GetPathToPartition(targetPartition, 4);
        if(dijkstraPath == null)
        {
            return false;
        }
        else{
            return true;
        }
    }
    public void PathToTarget(Vector3 target)
    {
        arrivedAtDestination = false;
        partitionPath = new List<Vector2Int>();
        Vector2Int targetPartition = PartitionSystem.instance.WorldToPartitionCoords(target);
        partitionPath = GetPathToPartition(targetPartition, 4);
        StartCoroutine(i_PathToTarget());
    }
    private IEnumerator i_PathToTarget()
    {
        //Will iterate through each checkpoint on the path list. 
        currentPath = new List<Vector3>();
        foreach(Vector2Int pos in partitionPath)
        {
            Vector2Int partitionCoord = pos + currPartition;
            Vector3 coord = PartitionSystem.instance.PartitionToWorldCoords(partitionCoord);
            coord.y = transform.position.y;
            currentPath.Add(coord);
        }
        Vector3 destination = currentPath[currentPath.Count -1];
        //Iterate through each path
        for(int i =0; i < currentPath.Count; i++)
        {
            Vector3 target =  currentPath[i];
            float distance;
            do
            {
                Vector2 currPos = new Vector2(transform.position.x,transform.position.z);
                Vector2 targetPos = new Vector2(target.x,target.z);
                distance = Vector2.Distance(currPos, targetPos);
                Vector2 targetVelocity = targetPos - currPos;
                targetVelocity.Normalize();
                velocity = new Vector3(targetVelocity.x,0,targetVelocity.y);
                
                yield return new WaitForEndOfFrame();
            }
            while(distance > 0.1);
        }
        velocity = Vector3.zero;
        partitionPath.Clear();
        arrivedAtDestination = true;
        yield return null;
    }
    private List<Node> GetAdjacentNodes(Vector2Int nodePos)
    {
        List<Node> adjacentNodes = new List<Node>();
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++)
            {
                Vector2Int displacement = new Vector2Int(i,j);
                if(displacement == Vector2Int.zero || 
                   displacement == new Vector2Int(1,1) ||
                   displacement == new Vector2Int(-1,-1)||
                   displacement == new Vector2Int(-1,1)||
                   displacement == new Vector2Int(1,-1))
                {
                    continue;
                }
                Vector2Int coords = nodePos + displacement;
                if(coords.x >= 0 && coords.x < gridSize.x &&
                   coords.y >= 0 && coords.y < gridSize.y)
                {
                    adjacentNodes.Add(nodes[coords.x,coords.y]);
                }
            }
        }
        return adjacentNodes;
    }
    #endregion
}
