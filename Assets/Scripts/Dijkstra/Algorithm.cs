using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(5,5);
    public Vector2Int start, end;

    private Node[,] nodes;
    private List<Node> unvisitedNodes;
    private List<Node> unfilteredNodes;
    void Awake()
    {
        GenerateNodes();
    }
    void GenerateNodes()
    {
        unvisitedNodes = new List<Node>();
        unfilteredNodes = new List<Node>();
        nodes = new Node[5,5];
        for(int i =0; i < 5; i++){
            for(int j = 0; j < 5; j++)
            {
                Vector2Int pos = new Vector2Int(i,j);
                Node newNode = new Node(pos, true);
                nodes[i,j] = newNode;
                unfilteredNodes.Add(newNode);
            }
        }  
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ComputeDijkstra();
        }
    }
    void ComputeDijkstra()
    {
        //Intialise vals
        Node startNode, endNode;
        startNode = nodes[start.x,start.y];
        endNode = nodes[end.x,end.y];
        Node currNode = startNode;
        Node neigbourNode;
        startNode.distanceFromRoot = 0;
        List<Node> neigbhourNodes = getAdjacentNodes(currNode.pos);


        while(unvisitedNodes.Count > 0)
        {
            neigbourNode = unvisitedNodes[0];
            float distance = Vector2Int.Distance(neigbourNode.pos, currNode.pos);
            if(currNode.distanceFromRoot + distance < neigbourNode.distanceFromRoot)
            {
                neigbourNode.distanceFromRoot = currNode.distanceFromRoot + distance;
                neigbourNode.rootNode = currNode.pos;
            }
        }
    }
    private List<Node> getAdjacentNodes(Vector2Int nodePos)
    {
        List<Node> adjacentNodes = new List<Node>();
        for(int i = -1; i < 1; i++){
            for(int j = -1; j < 1; j++)
            {
                Vector2Int displacement = new Vector2Int(i,j);
                if(displacement == Vector2Int.zero)
                {
                    continue;
                }
                Vector2Int coords = nodePos + displacement;
                if(coords.x > 0 && coords.x <= size.x &&
                   coords.y > 0 && coords.y <= size.y)
                {
                        adjacentNodes.Add(nodes[coords.x,coords.y]);
                }
            }
        }
        return adjacentNodes;
    }
    void OnDrawGizmos()
    {
        if(nodes != null)
        {
            
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    Gizmos.color = Color.white;
                    Vector2Int node = new Vector2Int(i,j);
                    if(node == start)
                    {
                        Gizmos.color = Color.green;
                    }
                    else if(node == end)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if(nodes[i,j].visited)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    Vector3 pos = new Vector3(i,0,j);
                    Gizmos.DrawWireCube(transform.position + pos, Vector3.one * 0.5f);
                }
            }
        }
    }
}
