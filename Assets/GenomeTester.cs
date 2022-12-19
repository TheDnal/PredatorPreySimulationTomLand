using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenomeTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Genome A = GeneticsSystem.GetStartingGenome(Genome.species.prey);
        Genome B = GeneticsSystem.GetStartingGenome(Genome.species.prey);

        B.rK_Value = 0.25f;
        Genome C = GeneticsSystem.MergeGenome(A,B);
        Debug.Log(A.respirationRate + " " + B.respirationRate + " " + C.respirationRate);
    }
}
