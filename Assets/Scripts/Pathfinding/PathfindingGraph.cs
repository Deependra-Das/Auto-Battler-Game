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

    public List<Node> GetShortestPath(Node source, Node destination)
    {
        List<Node> path = new List<Node>();

        if (source == destination)
        {
            path.Add(source);
            return path;
        }

        var openSet = new List<Node> { source };
        var closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (var node in _nodeList)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[source] = 0f;
        fScore[source] = Heuristic(source, destination);

        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(n => fScore[n]).First();

            if (current == destination)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in GetNeighbours(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                if (neighbor.IsOccupied && neighbor != destination)
                    continue;

                float tentativeG = gScore[current] +
                                   Vector3.Distance(current.worldPosition, neighbor.worldPosition);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeG >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, destination);
            }
        }

        return path;
    }

    private float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.worldPosition, b.worldPosition);
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node>();
        totalPath.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }

    public void ClearAllNodeOccupancy()
    {
        foreach (Node node in _nodeList)
        {
            node.SetOccupied(false);
        }
    }
}
