using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphService
{
    protected PathFindingGraph graph;
    public PathFindingGraph Graph => graph;

    private List<Tile> tileList = new List<Tile>();

    public GraphService(List<Tile> spawnedTiles)
    {
        tileList = spawnedTiles;
        InitializeGraph();
    }

    private void InitializeGraph()
    {
        graph = new PathFindingGraph();

        for (int i = 0; i < tileList.Count; i++)
        {
            Vector3 place = tileList[i].transform.position;
            graph.AddNode(place);
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
}