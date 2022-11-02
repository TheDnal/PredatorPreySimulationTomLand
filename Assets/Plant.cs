using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private PlantSpawner spawner;
    private bool isBeingEaten = false;
    void Start()
    {
        spawner = PlantSpawner.instance;
    }
    public bool isEdible()
    {
        return isBeingEaten;
    }
    public void startEating()
    {
        isBeingEaten = true;
    }
    public void stopEating()
    {
        isBeingEaten = false;
    }
    public void Consume()
    {
        spawner.DecrementPlantCount();
        Destroy(this.gameObject);
    }
}
