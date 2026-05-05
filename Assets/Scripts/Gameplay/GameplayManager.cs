using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    protected PathFindingGraph graph;
    TileGridService tileGridService;
    GraphService graphService;
    TeamService teamService;
    InventoryService inventoryService;
    BuffService buffService;
    StageService stageService;
    private Dictionary<int, UnitData> _unitDatabase = new Dictionary<int, UnitData>();

    public int fromIndex = 0;
    public int toIndex = 0;

    private int _currentStage = -1;
    private int _currentRound = -1;

    public GameplayStateEnum CurrentGameplayState { get; private set; } = GameplayStateEnum.Preparation;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(UnitScriptableObject unit_SO)
    {
        UIManager.Instance.InitializeGameplayUI();

        tileGridService = GameManager.Instance.Get<TileGridService>();
        graphService = GameManager.Instance.Get<GraphService>();
        teamService = GameManager.Instance.Get<TeamService>();
        inventoryService = GameManager.Instance.Get<InventoryService>();
        buffService = GameManager.Instance.Get<BuffService>();
        stageService = GameManager.Instance.Get<StageService>();

        graphService.Initialize(tileGridService.GetSpawnedTilesList());
        graph = graphService.Graph;

        buffService.InitializeBuffs();
        BuildUnitDatabase(unit_SO);
        inventoryService.SetMaxInventorySize(8);
        InitializeStageForGameplay(0);
    }

    private void BuildUnitDatabase(UnitScriptableObject unit_SO)
    {
        _unitDatabase = unit_SO.unitDataList.ToDictionary(unit => unit.unitID, unit => unit);
    }

    public void InitializeStageForGameplay(int stageIndex)
    {
        stageService.StartStage(stageIndex);
        _currentStage = stageService.CurrentStageIndex;
        _currentRound = stageService.CurrentRoundIndex;
        GameManager.Instance.Get<ShopService>().GenerateShopUnits();
        PrepareTeam2UnitsForRound();
        InstantiateTeam2Units();
    }

    public void InstantiateUnit(UnitData unitData, Node node, TeamEnum team )
    {
        BaseUnit newUnit = Instantiate(unitData.unitPrefab);
        newUnit.Initialize(unitData, team, node);

        teamService.MoveToField(newUnit, team);
        inventoryService.RemoveUnit(newUnit.UnitData);
    }

    private void PrepareTeam2UnitsForRound()
    {
        List<RoundEnemyData> enemiesForRound = stageService.GetCurrentRoundData().enemiyList;
        int enemyCount = enemiesForRound.Count;
        teamService.SetFieldCapacity(TeamEnum.Team2, enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            RoundEnemyData enemy = enemiesForRound[i];

            if (_unitDatabase.TryGetValue(enemy.enemyID, out UnitData enemyUnitData))
            {
                enemyUnitData.unitLevel = enemy.enemyLevel;
                teamService.AddUnitToTeam(enemyUnitData, TeamEnum.Team2);
            }
            else
            {
                Debug.LogWarning($"Enemy ID {enemy.enemyID} not found in database.");
            }
        }
    }

    private void InstantiateTeam2Units()
    {
        int team2FieldCapacity = teamService.GetFieldCapacity(TeamEnum.Team2);

        IReadOnlyList<UnitData> team2Units = teamService.GetTeamUnits(TeamEnum.Team2);
        for (int i = 0; i < team2FieldCapacity; i++)
        {
            UnitData unitData = team2Units[i];
            BaseUnit newUnit = Instantiate(unitData.unitPrefab);
            newUnit.Initialize(unitData, TeamEnum.Team2, graphService.GetUnOccupiedNode(TeamEnum.Team2));
            teamService.MoveToField(newUnit, TeamEnum.Team2);
            inventoryService.RemoveUnit(newUnit.UnitData);
        }
    }

    public IReadOnlyList<BaseUnit> GetOpponentTeamUnits(TeamEnum opponentTeam)
    {
        if (opponentTeam == TeamEnum.Team1)
        {
            return teamService.GetFieldUnits(TeamEnum.Team2);
        }
        else
        {
            return teamService.GetFieldUnits(TeamEnum.Team1);
        }
    }

    public void MarkUnitDead(BaseUnit unit)
    {
        teamService.RemoveUnitFromField(unit, unit.Team);
        Destroy(unit.gameObject);
        StartCoroutine(CheckRoundEnd());
    }

    private IEnumerator CheckRoundEnd()
    {
        yield return new WaitForSeconds(1f);
        bool team1 = TeamHasNoUnits(TeamEnum.Team1);
        bool team2 = TeamHasNoUnits(TeamEnum.Team2);

        if (team1 && team2) Debug.Log("Draw");
        else if (team1 && !team2) Debug.Log("Team2");
        else if (!team1 && team2) Debug.Log("Team1");
    }

    private bool TeamHasNoUnits(TeamEnum team)
    {
        return teamService.GetFieldUnitsCount(team) == 0;
    }

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

    public void UpdateGameplayState(GameplayStateEnum newState)
    {
        if (CurrentGameplayState == newState)
            return;

        CurrentGameplayState = newState;

        EventBusManager.Instance.Raise(EventNameEnum.GameplayStateChanged, CurrentGameplayState);
    }
}