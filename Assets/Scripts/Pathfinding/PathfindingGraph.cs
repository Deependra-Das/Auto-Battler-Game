using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingGraph
{
    private List<Node> _nodeList;
    private List<Path> _pathList;
    private float _defaultWeight = 1.0f;

    public List<Node> Nodes => _nodeList;
    public List<Path> Paths => _pathList;

    public PathFindingGraph()
    {
        _nodeList = new List<Node>();
        _pathList = new List<Path>();
    }

    public Node AddNode(Vector3 worldPosition)
    {
        Node node = new Node(_nodeList.Count, worldPosition);
        _nodeList.Add(node);
        return node;
    }

    public void AddPath(Node source, Node destination)
    {
        _pathList.Add(new Path(source, destination, _defaultWeight));
    }

    public bool CheckAdjacent(Node source, Node destination)
    {
        foreach (Path path in _pathList)
        {
            if (path.source == source && path.destination == destination)
                return true;
        }
        return false;
    }

    public List<Node> GetNeighbours(Node source)
    {
        List<Node> neighBourList = new List<Node>();

        foreach (Path path in _pathList)
        {
            if (path.source == source)
                neighBourList.Add(path.destination);
        }

        return neighBourList;
    }

    public float GetPathWeight(Node source, Node destination)
    {
        foreach (Path path in _pathList)
        {
            if (path.source == source && path.destination == destination)
                return path.GetWeight();
        }

        return Mathf.Infinity;
    }

    public virtual List<Node> GetShortestPath(Node source, Node destination)
    {
        List<Node> path = new List<Node>();

        if (source == destination)
        {
            path.Add(source);
            return path;
        }

        List<Node> unvisited = new List<Node>();
        Dictionary<Node, Node> predecessor = new Dictionary<Node, Node>();
        Dictionary<Node, float> distances = new Dictionary<Node, float>();

        foreach (Node node in _nodeList)
        {
            unvisited.Add(node);
            distances[node] = float.MaxValue;
        }

        distances[source] = 0f;

        while (unvisited.Count > 0)
        {
            unvisited = unvisited.OrderBy(node => distances[node]).ToList();
            Node currentNode = unvisited[0];
            unvisited.Remove(currentNode);

            if (currentNode == destination)
            {
                while (predecessor.ContainsKey(currentNode))
                {
                    path.Insert(0, currentNode);
                    currentNode = predecessor[currentNode];
                }

                path.Insert(0, currentNode);
                break;
            }

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                float length = Vector3.Distance(currentNode.position, neighbor.position);
                float alternateDistance = distances[currentNode] + length;

                if (alternateDistance < distances[neighbor])
                {
                    distances[neighbor] = alternateDistance;
                    predecessor[neighbor] = currentNode;
                }
            }
        }

        return path;
    }
}
