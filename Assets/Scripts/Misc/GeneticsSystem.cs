using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneticsSystem
{
    public static Genome GetStartingPreyGenome()
    {
        Genome newGenome = new Genome(0f, 1, 1, 2, -.5f, 1, 1);
        return newGenome;
    }

}
public struct Genome
{
    #region Genes
    public float rK_Value; 
    public float speed;
    public float size;
    public float respirationRate;
    public float visionRadius;
    public float visionAngle;
    public float smellRadius;
    public float hearingRadius;
    #endregion 
    public Genome(float rK = 0f, float _speed = 1f, float _size = 1f, 
                      int _visionRadius = 2, float _visionAngle = -.5f,
                      int _smellRadius = 1, int _hearingRadius = 1)
    {
        rK_Value = rK;
        speed = _speed;
        size = _size;
        respirationRate = 1;
        visionRadius = _visionRadius;
        visionAngle = _visionAngle;
        smellRadius = _smellRadius;
        hearingRadius = _hearingRadius;
        respirationRate = CalculateRespirationRate();
    }
    private float CalculateRespirationRate()
    {
        //Calculates respiration based of speed value, size, vision angle, and sensory radii
        float sensoryCost = 0.05f * (visionRadius + smellRadius + hearingRadius);
        sensoryCost += Mathf.Abs(visionRadius/5);
        float bodyCost = size /5 + speed /5;
        float respirationRate = sensoryCost + bodyCost;
        return respirationRate;
    }
}
public struct PredatorGenome
{

}