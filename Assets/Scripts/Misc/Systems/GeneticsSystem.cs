using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneticsSystem
{
    public static Genome GetStartingGenome(Genome.species species)
    {
        //prey genome
        if(species == Genome.species.prey)
        {
            Genome startingPreyGenome = new Genome(0f,1,1,2,-.5f,2,2, 0, Genome.species.prey);
            return MergeGenome(startingPreyGenome,startingPreyGenome);
        }
        //predator genome
        else
        {
            Genome startingPredatorGenome = new Genome(0f,1,1.5f,4,-.6f,4,4, 1, Genome.species.predator);
            return MergeGenome(startingPredatorGenome,startingPredatorGenome);
        }
    }
    public static Genome MergeGenome(Genome parentA, Genome parentB)
    {
        Genome newGenome = new Genome(
            MutateValue(parentA.rK_Value,parentB.rK_Value,0.25f,-1,1),
            MutateValue(parentA.speed,parentB.speed,0.25f,0.5f,3f),
            MutateValue(parentA.size,parentB.size,0.25f,0.5f,2f),
            Mathf.RoundToInt( MutateValue(parentA.visionRadius,parentB.visionRadius,1.5f,0,5)),
            MutateValue(parentA.visionAngle,parentB.visionAngle,0.25f,-.7f,1),
            Mathf.RoundToInt(MutateValue(parentA.smellRadius, parentB.smellRadius, 1.5f, 0, 3)),
            Mathf.RoundToInt( MutateValue(parentA.hearingRadius,parentB.hearingRadius,1.5f,0,5)),
            1,
            parentA.genomeSpecies
        );
        if(parentA.genomeSpecies == Genome.species.prey || parentB.genomeSpecies == Genome.species.prey)
        {
            newGenome.intelligence = 0;
        }
        return newGenome;
    }
    private static float MutateValue(float parentAvalue, float parentBvalue, float mutationStrength, float minValue, float maxValue)
    {
        float randomness = Random.Range(-mutationStrength, mutationStrength);
        float value = parentAvalue + parentBvalue;
        value *= 0.5f;
        value += randomness;
        if(value < minValue){value = minValue;}
        else if(value > maxValue){value = maxValue;}
        return RoundToTwoDecimalPlaces(value);
    }
    private static float RoundToTwoDecimalPlaces(float value)
    {
        float rounded = value * 100;
        rounded = Mathf.RoundToInt(rounded);
        rounded /= 100;
        return rounded;
    }
}
public struct Genome
{
    #region Genes
    public enum species{prey,predator}
    public species genomeSpecies;
    public float rK_Value; 
    public float speed;
    public float size;
    public float respirationRate;
    public int visionRadius;
    public float visionAngle;
    public int smellRadius;
    public float hearingRadius;
    public int intelligence;
    #endregion 
    public Genome(float rK = 0f, float _speed = 1f, float _size = 1f, 
                      int _visionRadius = 2, float _visionAngle = -.5f,
                      int _smellRadius = 1, int _hearingRadius = 1, int _intelligence = 0, species _species = species.prey)
    {
        rK_Value = rK;
        speed = _speed;
        size = _size;
        respirationRate = 1;
        visionRadius = _visionRadius;
        visionAngle = _visionAngle;
        smellRadius = _smellRadius;
        hearingRadius = _hearingRadius;
        genomeSpecies = _species;
        intelligence = _intelligence;
        respirationRate = CalculateRespirationRate();
    }
    private float CalculateRespirationRate()
    {
        //Calculates respiration based of speed value, size, vision angle, and sensory radii
        float sensoryCost = 0.05f * (visionRadius + smellRadius + hearingRadius);
        sensoryCost += Mathf.Abs(visionRadius/5);
        float bodyCost = size /5 + speed /5;
        float intelligenceCost = 0.1f * intelligence;
        float respirationRate = sensoryCost + bodyCost + intelligenceCost;
        return respirationRate;
    }
}