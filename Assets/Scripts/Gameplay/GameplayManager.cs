using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private TileScriptableObjectScript _tile_SO;

    protected PathFindingGraph graph;

    void Start()
    {
        TileGridService tileGridService = new TileGridService(_tile_SO);

        GraphService graphService = new GraphService(tileGridService.GetSpawnedTilesList());
        graph = graphService.Graph;
    }

    public int fromIndex = 0;
    public int toIndex = 0;

    private void OnDrawGizmos()
    {
        if (graph == null) return;

        var PathList = graph.Paths;

        if (PathList == null) return;

        foreach (Path path in PathList)
        {
            Debug.DrawLine(path.source.position, path.destination.position, Color.black, 100);
        }

        var NodesList = graph.Nodes;

        if (NodesList == null)
            return;

        foreach (Node node in NodesList)
        {
            Gizmos.color = node.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(node.position, 0.1f);

        }

        if (fromIndex >= NodesList.Count || toIndex >= NodesList.Count)
            return;

        List<Node> pathList = graph.GetShortestPath(NodesList[fromIndex], NodesList[toIndex]);

        if (pathList.Count > 1)
        {
            for (int i = 1; i < pathList.Count; i++)
            {
                Debug.DrawLine(pathList[i - 1].position, pathList[i].position, Color.red, 10);
            }
        }
    }
}
