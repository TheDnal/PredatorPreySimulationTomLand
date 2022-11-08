using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int pos;
    public bool traversable;
    public float distance;
    public Vector2Int rootNode;
    public Vector2Int chainNode;
    public bool visited;
    public Node(Vector2Int _pos, bool _traversable)
    {   
        pos = _pos;
        traversable = _traversable;
        distance = float.MaxValue;
        chainNode = pos; 
        visited = false;
    }
}   
