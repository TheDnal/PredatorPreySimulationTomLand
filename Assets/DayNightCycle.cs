using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    /*
        This script controls the day night cycle of the simulation
    */
    public float cycleDuration = 20f;
    private float rotationalVelocity = 0;
    void Start()
    {
        rotationalVelocity = 360 / 20f;
    }
    void Update()
    {
        this.transform.Rotate(new Vector3(rotationalVelocity * Time.deltaTime, 0, 0), Space.World); 
    }
}
