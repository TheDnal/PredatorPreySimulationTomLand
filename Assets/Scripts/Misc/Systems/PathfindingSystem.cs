using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingSystem : MonoBehaviour
{
    #region Fields
    private Node[,] nodes;
    Vector2Int gridSize;
    Vector2Int startPos,endPos;
    Node startNode,endNode;
    Vector2Int gridCorner;
    private List<Vector2Int> partitionPath = new List<Vector2Int>();
    private GOPAgent agent;
    private List<Vector2Int> gizmoPath = new List<Vector2Int>();
    private Vector2Int endGizmo, startGizmo;
    #endregion
    #region Algorithm
    public void Initialise(GOPAgent _agent)
    {
        agent = _agent;
    }
    public List<Vector2Int> GetPathToPartition(Vector2Int endPartition, int pathfindingRadius)
    {
        Vector2Int bounds = PartitionSystem.instance.size;
        if(endPartition.x < 0 || endPartition.x >= bounds.x || endPartition.y < 0|| endPartition.y >= bounds.y)
        {
            Debug.Log("out of bounds");
            return null;
        }
        
        if(!PartitionSystem.instance.partitions[endPartition.x,endPartition.y].IsTraversable())
        {
            Debug.Log("non traversable path");
            return null;
        }
        startPos = agent.getCurrPartition();
        endPos = endPartition;
        GenerateNodeGrid(pathfindingRadius, endPos);
        partitionPath = DijkstraCompute();
        gizmoPath = partitionPath;
        return partitionPath;
    }
    private void GenerateNodeGrid(int radius, Vector2Int end)
    {
        //Length of node grid
        int length = 1 + 2* radius;
        //Generate node grid
        nodes = new Node[length,length];

        gridSize = new Vector2Int(length,length);
        //Get grid corner coord, in world coords
        gridCorner = startPos - new Vector2Int(radius,radius);
        //Get end coord in relative coords
        Vector2Int endCoord = end - gridCorner;

        Vector2Int bounds = PartitionSystem.instance.size;
        //Generate node grid, starting from the grid corner
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < length; j++)
            {
                Vector2Int pos = new Vector2Int(i,j);
                bool traversable;
                Vector2Int worldPos = pos + gridCorner;
                if(worldPos.x < 0 || worldPos.x >= bounds.x || worldPos.y < 0|| worldPos.y >= bounds.y)
                {
                    traversable = false;
                }
                else
                {
                    traversable = PartitionSystem.instance.partitions[worldPos.x,worldPos.y].IsTraversable();
                }
                Node newNode = new Node(pos,traversable);
                nodes[i,j] = newNode;
            }
        }
        //Get start and end node
        startNode = nodes[radius,radius];
        endNode = nodes[endCoord.x,endCoord.y];
    }
    private List<Vector2Int> DijkstraCompute()
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

        partitionPath = new List<Vector2Int>();
        //Get start node
        Node currentNode = startNode;
        currentNode.distance = 0;
        List<Node> nodesToCompute = new List<Node>();
        //Get Adjacent nodes
        nodesToCompute.AddRange(GetAdjacentNodes(currentNode.pos));
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
        int x =0;
        //Repeat until no nodes left or reach end node visited
        while(nodesToCompute.Count > 0)
        {
            float distance;
            List<Node> nextNodesToCompute = new List<Node>();
            foreach(Node adjacentNode in nodesToCompute)
            {
                x++;
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
                    List<Node> adjacentNodes = GetAdjacentNodes(adjacentNode.pos);
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
        if(!endNode.visited){
            return null;
        }
        List<Vector2Int> reversePath = new List<Vector2Int>();
        Vector2Int currStep = endNode.pos;
        x = 0;
        while(currStep != startNode.pos && x < 99)
        {
            x++;
            reversePath.Add(currStep);
            currStep = nodes[currStep.x,currStep.y].chainNode;
        }
        reversePath.Add(startNode.pos);
        partitionPath = new List<Vector2Int>();

        for(int i = reversePath.Count - 1; i >=0 ; i--)
        {
            partitionPath.Add(reversePath[i] - startNode.pos);
        }
        return partitionPath;
    }
    
    public bool isTargetReachable(Vector3 target)
    {
        //Actions will use this to make sure their target location is valid
        Vector2Int targetPartition = PartitionSystem.instance.WorldToPartitionCoords(target);
        List<Vector2Int> dijkstraPath = GetPathToPartition(targetPartition, 4);
        if(dijkstraPath == null)
        {
            return false;
        }
        else{
            return true;
        }
    }
    private List<Node> GetAdjacentNodes(Vector2Int nodePos)
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
                if(coords.x >= 0 && coords.x < gridSize.x &&
                   coords.y >= 0 && coords.y < gridSize.y)
                {
                    adjacentNodes.Add(nodes[coords.x,coords.y]);
                }
            }
        }
        return adjacentNodes;
    }
   
    #endregion
}
