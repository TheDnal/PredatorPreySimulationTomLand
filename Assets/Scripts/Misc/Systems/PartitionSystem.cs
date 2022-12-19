using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartitionSystem : MonoBehaviour
{
    public static PartitionSystem instance;
    private Vector3 startPos;
    public Vector2Int size;
    private float partitionSize;
    public Partition[,] partitions;
    public enum ObjectType{predator, prey, food};
    private bool initialised = false;
    public Gradient scoreRange;
    public float multiplier = 0;
    public enum DebugType { none, food, drinkableWater, bodyOfWater, agents, score, danger, sound};
    public DebugType GizmosMode;
    #region Initialization and generation of spacial partitioning system
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }
    public void Initialise(Vector3 _startPos, Vector2Int _size, float _partitionSize)
    {
        startPos = _startPos;
        size = _size;
        partitionSize = _partitionSize;
        partitions = new Partition[size.x,size.y];
        for(int i =0 ; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                Vector3 displacement = new Vector3(i,0,j) * partitionSize;
                Vector2Int coords = new Vector2Int(i,j);
                bool water = MapGenerator.instance.isTileUnderWater(new Vector2Int(i,j));
                float height = MapGenerator.instance.GetTileHeight(new Vector2Int(i,j));
                bool traversable = MapGenerator.instance.isTileTraversable(new Vector2Int(i,j));
                Partition newPart = new Partition(startPos + displacement, coords, water, false, traversable);
                newPart.SetHeight(height);
                partitions[i,j] = newPart;
                
            }
        }
        foreach(Partition p in partitions)
        {
            p.UpdateDrinkableStatus();
        }
        initialised =true;
        StartCoroutine(i_ScoreCalculator());
    }
    public void Update()
    {
        if(!initialised){return;}
        foreach(Partition p in partitions)
        {
            p.UpdateNoises();
        }
    }
    #endregion
    #region Public methods
    public void AddGameObjectToPartition(GameObject _GameObject, ObjectType type)
    {
        Vector2Int partitionCoords = WorldToPartitionCoords(_GameObject.transform.position);
        if(type == ObjectType.predator)
        {
            partitions[partitionCoords.x,partitionCoords.y].AddAgent(_GameObject, false);
            return;
        }
        else if(type == ObjectType.prey)
        {
            partitions[partitionCoords.x,partitionCoords.y].AddAgent(_GameObject, true);
            return;
        }
        else if(type == ObjectType.food)
        {
            partitions[partitionCoords.x,partitionCoords.y].AddFood(_GameObject);
            return;
        }
        return;
    }   
    public void RemoveGameObjectFromPartition(GameObject _GameObject, Vector2Int partitionPos, ObjectType type)
    {
        if(type == ObjectType.predator)
        {
            if( !partitions[partitionPos.x,partitionPos.y].agents.Contains(_GameObject))
            {
                return;
            }
            partitions[partitionPos.x,partitionPos.y].RemoveAgent(_GameObject, false);
            return;
        }
        else if(type == ObjectType.prey)
        {
            if(!partitions[partitionPos.x,partitionPos.y].agents.Contains(_GameObject))
            {
                return;
            }
            partitions[partitionPos.x,partitionPos.y].RemoveAgent(_GameObject, true);
        }
        else if(type == ObjectType.food)
        {
            if( !partitions[partitionPos.x,partitionPos.y].food.Contains(_GameObject))
            {
                return;
            }
            partitions[partitionPos.x,partitionPos.y].RemoveFood(_GameObject);
            return;
        }

       
    }
    public Vector2Int WorldToPartitionCoords(Vector3 position)
    {
        Vector3 roundedPos = new Vector3(Mathf.RoundToInt(position.x), 0, 
                                         Mathf.RoundToInt(position.z));

        Vector3 relativePosition = roundedPos - startPos;
        Vector2Int partitionPos =  new Vector2Int(Mathf.RoundToInt(relativePosition.x), Mathf.RoundToInt(relativePosition.z));
        return partitionPos;
    }
    public Vector3 PartitionToWorldCoords(Vector2Int coords)
    {
        Vector3 offset = new Vector3(coords.x, 0 , coords.y);
        Vector3 position = startPos + offset;   
        return position;
    }
    public List<Partition> GetPartitionsInRadius(Vector3 position, int radius)
    {
        //Gets list of adjacent partitions
        List<Partition> partitionsInRadius = new List<Partition>();
        Vector2Int coreCoords = WorldToPartitionCoords(position);
        for(int i= -radius; i <= radius; i++)
        {
            if(coreCoords.x + i < 0 || coreCoords.x + i >= size.x){continue;}
    
            for(int j= -radius; j <= radius; j++)
            {
                if(coreCoords.y + j < 0 || coreCoords.y + j >= size.y){continue;}
                partitionsInRadius.Add(partitions[coreCoords.x + i, coreCoords.y + j]);
            }
        }
        return partitionsInRadius;
    }
    public Vector3 GetValidSpawnZone()
    {
        List<Vector3> validSpawns = new List<Vector3>();
        foreach(Partition p in partitions)
        {
            if(!p.IsWater() && p.foodCount < 3 && p.IsTraversable())
            {
                validSpawns.Add(p.worldPosition);
            }
        }
        int index = Random.Range(0, validSpawns.Count - 1);
        return validSpawns[index];
    }
    public List<Vector3> GetValidSpawnZones()
    {
        List<Vector3> validSpawns = new List<Vector3>();
        foreach(Partition p in partitions)
        {
            if(!p.IsWater() && p.foodCount < 3 && p.IsTraversable())
            {
                validSpawns.Add(p.worldPosition);
            }
        }
        return validSpawns;
    }
    public void EmitSound(int soundRadius ,Vector3 soundSource, noise.noiseType soundType)
    {
        noise newNoise = new noise(soundType, soundSource);
        Vector2Int partitionPos = WorldToPartitionCoords(soundSource);
        List<Partition> affectedPartitions = new List<Partition>();
        affectedPartitions.AddRange(GetPartitionsInRadius(soundSource,soundRadius));
        partitions[partitionPos.x,partitionPos.y].AddNoise(newNoise);
        foreach(Partition p in affectedPartitions)
        {
            p.AddNoise(newNoise);
        }
        return;
    }
    #endregion
    #region Misc/Private methods
    private IEnumerator i_ScoreCalculator()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            List<Partition> nearbyPartitions = new List<Partition>();
            foreach(Partition p in partitions)
            {
                Vector3 pos = PartitionToWorldCoords(p.coords);
                nearbyPartitions = GetPartitionsInRadius(pos, 2);
                float score = 0;
                score += p.GetFoodCount();
                score += p.IsWater() ? 0 : 1f - p.GetHeight();
                foreach(Partition adjacentP in nearbyPartitions)
                {
                    score += adjacentP.GetFoodCount();
                    score += adjacentP.IsWater() ? 1 : 0;
                }
                score = p.IsWater() ? 0 : score;
                p.SetScore(score);
            }
        }
    }
    void OnDrawGizmos()
    {
        if(partitions != null)
        {
            foreach(Partition p in partitions)
            {
                Vector3 pos = startPos + new Vector3(p.coords.x, 0, p.coords.y);
                switch(GizmosMode)
                {
                    case DebugType.none:
                        return;
                    case DebugType.food:
                        if(p.HasFood())
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                        }
                        break;
                    case DebugType.bodyOfWater:
                        if(p.IsWater())
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                        }
                        break;
                    case DebugType.drinkableWater:
                        if(p.hasDrinkbleWater())
                        {
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                        }
                        break;
                    case DebugType.agents:
                        if(p.agents.Count > 0)
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                        }
                        break;
                    case DebugType.score:
                        {
                            Color col = scoreRange.Evaluate(p.GetScore() * multiplier);
                            Gizmos.color = col;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                            break;
                        }
                    case DebugType.danger:
                        {
                            Color col = scoreRange.Evaluate(1 - p.GetDangerValue(true) * multiplier);
                            Gizmos.color = col;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                            break;
                        }
                    case DebugType.sound:
                        {
                            if(p.noises.Count == 0){break;}
                            float volume = 0;
                            foreach(noise n in p.noises)
                            {
                                volume += n.volume; 
                            }
                            Color col = scoreRange.Evaluate(volume * multiplier);
                            Gizmos.color = col;
                            Gizmos.DrawWireCube(pos + Vector3.up, Vector3.one * partitionSize);
                            break;
                        }   
                    default:
                        break;
                }
            }
        }
    }
    #endregion

}
public class Partition
{
    #region fields
    public List<noise> noises;
    public List<GameObject> agents;
    public List<GameObject> food;
    public Vector3 worldPosition;
    public Vector2Int coords;
    private bool isWater;
    private bool drinkableArea;
    private bool isTraversable;
    public int foodCount;
    private bool hasFood;
    private float score = 0;
    private float height;
    private float danger;
    private int dangerSources = 0;
    private int predatorCount = 0;
    #endregion
    //Constructor
    public Partition(Vector3 _Position, Vector2Int _coords, bool _isWater, bool _hasFood, bool _isTraversable)
    {
        food = new List<GameObject>();
        agents = new List<GameObject>();
        noises = new List<noise>();
        foodCount = 0;
        worldPosition = _Position;
        coords = _coords;
        isWater = _isWater;
        hasFood = _hasFood;
        isTraversable = _isTraversable;
        drinkableArea = false;
    }
    #region methods
    public void IncrementFoodCount(){foodCount++; hasFood = true;}
    public void DecrementFoodCount()
    {
        foodCount--;
        if(foodCount < 0)
        {
            foodCount = 0;
            hasFood = false;
        }
    }
    public void UpdateDrinkableStatus()
    {
        if(isWater)
        {
            drinkableArea = false;
            return;
        }
        Vector3 pos = PartitionSystem.instance.PartitionToWorldCoords(coords);
        List<Partition> nearby = PartitionSystem.instance.GetPartitionsInRadius(pos, 1);
        bool drinkable = false;
        foreach(Partition p in nearby)
        {
            if(p.IsWater())
            {
                drinkable = true;
            }
        }
        drinkableArea = drinkable;
    }
    public int GetFoodCount() {return foodCount;}
    public bool HasFood(){return foodCount > 0 ? true : false;}
    public bool hasDrinkbleWater(){return drinkableArea;}
    public bool IsWater(){return isWater;}
    public bool IsTraversable(){return isTraversable;}
    public void AddAgent(GameObject _agent, bool isPrey = true)
    {
        if(agents.Contains(_agent)){return;}
        if(!isPrey){ChangePredatorCount(1);}
        agents.Add(_agent);
    }
    public void RemoveAgent(GameObject _agent, bool isPrey = true)
    {
        if(!agents.Contains(_agent)){return;}
        if(!isPrey){ChangePredatorCount(-1);}
        agents.Remove(_agent);
    }
    public void AddFood(GameObject _food)
    {
        IncrementFoodCount();
        food.Add(_food);
    }
    public void RemoveFood(GameObject _food)
    {
        DecrementFoodCount();
        food.Remove(_food);
    }
    public float GetScore()
    {
        return score;
    }    
    public void SetScore(float _score)
    {
        score = _score;
    }    
    public float GetHeight()
    {
        return height;
    }
    public void SetHeight(float _height)
    {
        height = _height;
    }
    public void ChangeDangerSourceCount(int change)
    {
        dangerSources += change;
        dangerSources = dangerSources < 0 ? 0 : dangerSources;
    }
    public void ChangePredatorCount(int change)
    {
        predatorCount += change;
        predatorCount = predatorCount < 0 ? 0 : predatorCount;
    }
    public float GetDangerValue(bool isPrey)
    {
        float dangerValue = dangerSources;
        dangerValue += isPrey ? predatorCount : 0;
        return dangerValue;
    }
    public void AddNoise(noise _noise)
    {
        noises.Add(_noise);
    }
    public void UpdateNoises()
    {
        List<noise> noisesToRemove = new List<noise>();
        float decay = Time.deltaTime;
        foreach(noise currentNoise in noises)
        {
            currentNoise.changeVolume(-decay * 0.02f);
            if(currentNoise.volume <= 0)
            {
                noisesToRemove.Add(currentNoise);
            }
        }
        foreach(noise n in noisesToRemove)
        {
            noises.Remove(n);
        }
    }
    #endregion
}
public class noise
{
    public enum noiseType{deathScream, malePreyMatingCall, femalePreyMatingCall, malePredatorMatingCall, femalePredatorMatingCall}
    public noiseType type;
    public float volume;
    public Vector3 sourceLocation;
    public noise(noiseType _type, Vector3 _source)
    {
        type = _type;
        sourceLocation = _source;
        volume = 1;
    }
    public void changeVolume(float amount)
    {
        volume += amount;
    }
}