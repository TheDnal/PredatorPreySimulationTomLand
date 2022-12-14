using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public static CamController instance;
    public Vector3 cameraStartPos;
    public bool followAgent = false;
    private bool cullPOV = false;
    private GameObject currentAgent;
    public float speed = 5;
    private bool Initialised = false;
    // Start is called before the first frame update
    public void Awake()
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
    public void Initialise()
    {
        transform.position = cameraStartPos;
        Initialised = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Initialised){return;}
        if(Agent.selectedAgent != null)
        {       
            currentAgent = Agent.selectedAgent.GetGameObject();
        }
        if(followAgent && currentAgent != null)
        {
            Vector3 pos = transform.position;
            pos.x = currentAgent.transform.position.x;
            pos.z = currentAgent.transform.position.z;
            transform.position = pos;
        }
        if(cullPOV && currentAgent != null)
        {
            List<Partition> visiblePartitions = new List<Partition>();
            visiblePartitions.AddRange(Agent.selectedAgent.GetSensorySystem().GetVisionCone());
            visiblePartitions.AddRange(Agent.selectedAgent.GetSensorySystem().GetSmell());
            MapGenerator.instance.UpdateLayeredTiles(visiblePartitions);
            CullPartitions(visiblePartitions);
        }
        else
        {
            MapGenerator.instance.ClearAllLayeredTiles();
        }
        Vector3 velocity = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            velocity += Vector3.forward;
        }
        if(Input.GetKey(KeyCode.A))
        {
            velocity += Vector3.left;
        }
        if(Input.GetKey(KeyCode.S))
        {
            velocity += Vector3.back;
        }
        if(Input.GetKey(KeyCode.D))
        {
            velocity += Vector3.right;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            velocity += Vector3.up;
        }
        if(Input.GetKey(KeyCode.E))
        {
            velocity += Vector3.down;
        }
        velocity.Normalize();
        transform.position += velocity * Time.deltaTime * speed;
    }
    public void ToggleFollowingAgent()
    {
        if(Agent.selectedAgent == null){return;}
        currentAgent = Agent.selectedAgent.GetGameObject();
        followAgent = followAgent ? false : true;
    }
    public void ToggleCullPOV()
    {
        GameObject prevAgent = currentAgent;
        currentAgent = Agent.selectedAgent.GetGameObject();
        if(currentAgent == null)
        {
            return;
        }
        if(prevAgent != null)
        {
            if(prevAgent != currentAgent)
            {
                prevAgent.layer = 0;
            }
        }
        
        MapGenerator.instance.ClearAllLayeredTiles();
        cullPOV = cullPOV ? false : true;
        if(cullPOV)
        {
            this.GetComponent<Camera>().cullingMask = 1 << 8;
            currentAgent.layer = 8;
        }
        else
        {
            this.GetComponent<Camera>().cullingMask = -1;
            currentAgent.layer = 0;
        }
    }

    private void CullPartitions(List<Partition> partitions)
    {
        List<GameObject> entitiesToCull = new List<GameObject>();
        List<GameObject> foodToCull = new List<GameObject>();
        foreach(Partition p in partitions)
        {
            entitiesToCull.AddRange(p.agents);
            foodToCull.AddRange(p.food);
        }
        entitiesToCull.Add(currentAgent);
        EntitySpawner.instance.CullEntityList(entitiesToCull);
        PlantSpawner.instance.CullPlantList(foodToCull);
    }
}
