using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int pos;
    public bool traversable;
    public float distanceFromRoot;
    public Vector2Int rootNode;
    public bool visited;
    public Node(Vector2Int _pos, bool _traversable)
    {   
        pos = _pos;
        traversable = _traversable;
        distanceFromRoot = float.MaxValue;
        rootNode = pos; 
        visited = false;
    }
}   
