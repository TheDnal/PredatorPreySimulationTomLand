using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(5,5);
    public Vector2Int start, end;
    public Vector2Int obstacle;
    private Node[,] nodes;
    private List<Vector2Int> path = new List<Vector2Int>();
    void Awake()
    {
        Compute();
    }
    public void Compute()
    {
        GenerateNodes();
        ComputeDijkstra();
    }
    void GenerateNodes()
    {
        nodes = new Node[5,5];
        for(int i =0; i < 5; i++){
            for(int j = 0; j < 5; j++)
            {
                Vector2Int pos = new Vector2Int(i,j);
                bool _obstacle = true;
                if(pos == obstacle)
                {
                    _obstacle = false;
                }
                Node newNode = new Node(pos, _obstacle);
                nodes[i,j] = newNode;
            }
        }
    }
    void ComputeDijkstra()
    {
       /*
        Begin at start node
        get adjacent nodes
        get distance from start node to adjacent node. if dist + start node dist < adjacent dist,
        then adjacent dist = dist + start dist
        mark adjacent nodes as visited
        get all their neighbours (no repeats or visited nodes)
        repeat until no nodes left or reach end node visited
        */ 

        path = new List<Vector2Int>();
        //Get start node
        Node currentNode = nodes[start.x,start.y];
        currentNode.distance = 0;
        Node endNode = nodes[end.x,end.y];
        Node startNode = nodes[start.x,start.y];
        List<Node> nodesToCompute = new List<Node>();
        //Get Adjacent nodes
        nodesToCompute.AddRange(getAdjacentNodes(currentNode.pos));
        List<Node> nodesToPrune = new List<Node>();
        foreach(Node n in nodesToCompute)
        {
            if(!n.traversable)
            {
                nodesToPrune.Add(n);
            }
        }
        foreach(Node n in nodesToPrune)
        {
            nodesToCompute.Remove(n);
        }
        foreach(Node n in nodesToCompute)
        {
            n.rootNode = currentNode.pos;
        }
        //Repeat until no nodes left or reach end node visited
        int x= 0;
        while(nodesToCompute.Count > 0)
        {
            x++;
            float distance;
            List<Node> nextNodesToCompute = new List<Node>();
            foreach(Node adjacentNode in nodesToCompute)
            {
                currentNode = nodes[adjacentNode.rootNode.x, adjacentNode.rootNode.y];
                //Get distance from adjacent node to root node
                distance = Vector2Int.Distance(adjacentNode.pos, currentNode.pos);
                if(distance + currentNode.distance < adjacentNode.distance)
                {
                    //Set new distance
                    adjacentNode.distance = distance + currentNode.distance;
                    //Mark as visited
                    adjacentNode.visited = true;
                    adjacentNode.chainNode = currentNode.pos;
                    List<Node> adjacentNodes = getAdjacentNodes(adjacentNode.pos);
                    foreach(Node n in adjacentNodes)
                    {
                        if(!n.visited && !nextNodesToCompute.Contains(n) && n.traversable)
                        {
                            n.rootNode = adjacentNode.pos;
                            nextNodesToCompute.Add(n);
                        }
                    }
                }
            }
            nodesToCompute.Clear();
            nodesToCompute.AddRange(nextNodesToCompute);
        }
        List<Vector2Int> reversePath = new List<Vector2Int>();
        Vector2Int currStep = endNode.pos;
        x = 0;
        while(currStep != start && x < 99) 
        {
            x++;
            reversePath.Add(currStep);
            currStep = nodes[currStep.x,currStep.y].chainNode;
        }
        reversePath.Add(start);
        path = new List<Vector2Int>();
        for(int i = reversePath.Count - 1; i >=0 ; i--)
        {
            path.Add(reversePath[i]);
        }
    }
    private List<Node> getAdjacentNodes(Vector2Int nodePos)
    {
        List<Node> adjacentNodes = new List<Node>();
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++)
            {
                Vector2Int displacement = new Vector2Int(i,j);
                if(displacement == Vector2Int.zero || 
                   displacement == new Vector2Int(1,1) ||
                   displacement == new Vector2Int(-1,-1)||
                   displacement == new Vector2Int(-1,1)||
                   displacement == new Vector2Int(1,-1))
                {
                    continue;
                }
                Vector2Int coords = nodePos + displacement;
                if(coords.x >= 0 && coords.x < size.x &&
                   coords.y >= 0 && coords.y < size.y)
                {
                    
                        adjacentNodes.Add(nodes[coords.x,coords.y]);
                }
            }
        }
        return adjacentNodes;
    }
    private List<Node> pruneNodeList(List<Node> listToPrune)
    {
        //removes visited and untravserable nodes.
        if(listToPrune == null)
        {
            return listToPrune;
        }
        List<Node> PrunedList = listToPrune;
        List<Node> nodesToPrune = new List<Node>();
        foreach(Node n in listToPrune)
        {
            if(n.visited || !n.traversable)
            {
                nodesToPrune.Add(n);
            }
        }
        foreach(Node n in nodesToPrune)
        {
            PrunedList.Remove(n);
        }
        return PrunedList;
    }
    private List<Node> addUniqueNodes(List<Node> coreList, List<Node> nodesToAdd)
    {
        //Prevents duplicates from being added to a list
        foreach(Node n in nodesToAdd)
        {
            if(!coreList.Contains(n))
            {
                coreList.Add(n);
            }
        }
        return coreList;
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
                    else if(!nodes[i,j].traversable)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else if(path != null)
                    {
                        Vector2Int currNode = nodes[i,j].pos;
                        if(path.Contains(currNode))
                        {
                            Gizmos.color = Color.yellow;
                        }
                    }
                    Vector3 pos = new Vector3(i,0,j);
                    Gizmos.DrawWireCube(transform.position + pos, Vector3.one * 0.5f);
                }
            }
        }
    }
}
