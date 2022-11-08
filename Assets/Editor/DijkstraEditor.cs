using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Algorithm))]
public class DijkstraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Algorithm algo = (Algorithm)target;

        if(DrawDefaultInspector())
        {
            algo.Compute();
        }
    }
}
