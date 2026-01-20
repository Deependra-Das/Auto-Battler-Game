using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphService
{
    protected PathFindingGraph graph;
    public PathFindingGraph Graph => graph;

    private List<Tile> tileList = new List<Tile>();

    private Dictionary<TeamEnum, int> _startPositionForTeam;

    public void Initialize(List<Tile> spawnedTiles)
    {
        tileList = spawnedTiles;
        InitializeGraph();
        _startPositionForTeam = new Dictionary<TeamEnum, int>();
        _startPositionForTeam.Add(TeamEnum.Team1, 0);
        _startPositionForTeam.Add(TeamEnum.Team2, graph.Nodes.Count - 1);
    }

    private void InitializeGraph()
    {
        graph = new PathFindingGraph();

        for (int i = 0; i < tileList.Count; i++)
        {
            Vector3 place = tileList[i].transform.position;
            Node node = graph.AddNode(place);
            tileList[i].Node = node;
        }

        var allNodes = graph.Nodes;
        foreach (Node source in allNodes)
        {
            foreach (Node destination in allNodes)
            {
                if (Vector3.Distance(source.position, destination.position) < 1f && source != destination)
                {
                    graph.AddPath(source, destination);
                }
            }
        }
    }

    public Node GetUnOccupiedNode(TeamEnum team)
    {
        int startIndex = _startPositionForTeam[team];
        int currentIndex = startIndex;

        while (graph.Nodes[currentIndex].IsOccupied)
        {
            if(startIndex == 0)
            {
                currentIndex++;
                if (currentIndex == graph.Nodes.Count)
                    return null;
            }
            else
            {
                currentIndex--;
                if (currentIndex == -1)
                    return null;
            }
        }

        return graph.Nodes[currentIndex];
    }

    public List<Node> GetShortestPath(Node source, Node destination)
    {
        return graph.GetShortestPath(source, destination);
    }

    public List<Node> GetNodesCloseTo(Node destination)
    {
        return graph.GetNeighbours(destination);
    }
}