using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraphService
{
    private PathFindingGraph _graph;
    public PathFindingGraph Graph => _graph;

    private Tilemap _tilemap;

    private Dictionary<Vector3Int, Node> _cellToNode = new Dictionary<Vector3Int, Node>();

    public void InitializeGraph(Tilemap tilemap)
    {
        _cellToNode.Clear();

        _tilemap = tilemap;
        _graph = new PathFindingGraph();

        for (int x = _tilemap.cellBounds.xMin; x < _tilemap.cellBounds.xMax; x++)
        {
            for (int y = _tilemap.cellBounds.yMin; y < _tilemap.cellBounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (_tilemap.HasTile(cell))
                {
                    Vector3 worldPosition = _tilemap.GetCellCenterWorld(cell);
                    Node node = _graph.AddNode(worldPosition);
                    _cellToNode[cell] = node;
                }
            }
        }

        var allNodes = _graph.Nodes;
        foreach (Node source in allNodes)
        {
            foreach (Node destination in allNodes)
            {
                if (Vector3.Distance(source.worldPosition, destination.worldPosition) < 1f && source != destination)
                {
                    _graph.AddPath(source, destination);
                }
            }
        }
    }

    public List<Node> GetShortestPath(Node source, Node destination)
    {
        return _graph.GetShortestPath(source, destination);
    }

    public List<Node> GetNodesCloseTo(Node destination)
    {
        return _graph.GetNeighbours(destination);
    }

    public bool GetUnoccupiedNodeAtIndex(int index, out Node node)
    {
        if(_graph.Nodes[index].IsOccupied)
        {
            node = null;
            Debug.Log($"Node at Index {index} is Occupied");
            return false;
        }

        node = _graph.Nodes[index];
        return true;
    }
    public bool TryGetNodeAtCell(Vector3Int cell, out Node node)
    {
        return _cellToNode.TryGetValue(cell, out node);
    }

    public void ClearGraphNodeOccupancy()
    {
        _graph?.ClearAllNodeOccupancy();
    }

    public void Reset()
    {
        _cellToNode.Clear();
        _graph = null;
        _tilemap = null;
    }
}