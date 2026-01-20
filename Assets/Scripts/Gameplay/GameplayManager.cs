using AutoBattler.Main;
using AutoBattler.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : GenericMonoSingleton<GameplayManager>
{
    protected PathFindingGraph graph;
    TileGridService tileGridService;
    GraphService graphService;
    TeamService teamService;

    private List<UnitData> _unitPrefabList;

    protected override void Awake()
    {
        base.Awake();
        _unitPrefabList = new List<UnitData>();
    }

    public void Initialize(UnitScriptableObject unit_SO)
    {
        tileGridService = GameManager.Instance.Get<TileGridService>();
        graphService = GameManager.Instance.Get<GraphService>();
        teamService = GameManager.Instance.Get<TeamService>();

        graphService.Initialize(tileGridService.GetSpawnedTilesList());
        graph = graphService.Graph;

        _unitPrefabList = unit_SO.unitDataList;
        GameManager.Instance.Get<ShopService>().GenerateShopUnits();

        InstantiateTeam2Units();
    }

    public void InstantiateUnit(UnitData unitData, Node node, TeamEnum team )
    {
        BaseUnit newUnit = Instantiate(unitData.unitPrefab);
        newUnit.Initialize(team, node);
        teamService.AddUnitToTeam(newUnit, team);
    }

    private void InstantiateTeam2Units()
    {
        for (int i = 0; i < teamService.GetTeamCapacity(TeamEnum.Team2); i++)
        {
            UnitData randomUnitData = _unitPrefabList[Random.Range(0, _unitPrefabList.Count)];
            BaseUnit newUnit = Instantiate(randomUnitData.unitPrefab);
            newUnit.Initialize(TeamEnum.Team2, graphService.GetUnOccupiedNode(TeamEnum.Team2));
            teamService.AddUnitToTeam(newUnit, TeamEnum.Team2);
        }
    }

    public List<BaseUnit> GetOpponentTeamUnits(TeamEnum opponentTeam)
    {
        if (opponentTeam == TeamEnum.Team1)
            return teamService.GetTeamUnits(TeamEnum.Team2);
        else
            return teamService.GetTeamUnits(TeamEnum.Team1);
    }

    public void MarkUnitDead(BaseUnit unit)
    {
        teamService.RemoveUnitFromTeam(unit, unit.Team);
        Destroy(unit.gameObject);
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

        if (NodesList == null) return;

        foreach (Node node in NodesList)
        {
            Gizmos.color = node.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(node.position, 0.1f);
        }

        if (fromIndex >= NodesList.Count || toIndex >= NodesList.Count) return;

        List<Node> pathList = graph.GetShortestPath(NodesList[fromIndex], NodesList[toIndex]);

        if (pathList.Count > 1)
        {
            for (int i = 1; i < pathList.Count; i++)
            {
                Debug.DrawLine(pathList[i - 1].position, pathList[i].position, Color.red, 1);
            }
        }
    }
}