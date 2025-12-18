using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private TileScriptableObjectScript _tile_SO;
    [SerializeField] private List<BaseUnit> _unitPrefabList;
    protected PathFindingGraph graph;

    TileGridService tileGridService;
    GraphService graphService;
    TeamService teamService;

    void Start()
    {
        tileGridService = new TileGridService(_tile_SO);

        graphService = new GraphService(tileGridService.GetSpawnedTilesList());
        graph = graphService.Graph;

        teamService = new TeamService();
        InstantiateUnits();
    }

    private void InstantiateUnits()
    {
        for (int i = 0; i < teamService.GetTeamCapacity(TeamEnum.Team1); i++)
        {
            BaseUnit newUnit = Instantiate(_unitPrefabList[0]);
            teamService.AddUnitToTeam(newUnit, TeamEnum.Team1);
        }
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
