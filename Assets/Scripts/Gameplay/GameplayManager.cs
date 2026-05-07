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
    private TileGridService _tileGridServiceObj;
    private GraphService _graphServiceObj;
    private TeamService _teamServiceObj;
    private InventoryService _inventoryServiceObj;
    private BuffService _buffServiceObj;
    private StageService _stageServiceObj;
    private UnitService _unitServiceObj;

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
            InitializeGameplay();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeGameplay()
    {
        UIManager.Instance.InitializeGameplayUI();
        InventoryDropZoneManager.Instance.Initialize();

        _tileGridServiceObj = GameManager.Instance.Get<TileGridService>();
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _buffServiceObj = GameManager.Instance.Get<BuffService>();
        _stageServiceObj = GameManager.Instance.Get<StageService>();
        _unitServiceObj = GameManager.Instance.Get<UnitService>();

        _tileGridServiceObj.CreateTileMap();
        _graphServiceObj.Initialize(_tileGridServiceObj.GetSpawnedTilesList());
        graph = _graphServiceObj.Graph;

        _buffServiceObj.InitializeBuffs();
        _inventoryServiceObj.SetMaxInventorySize(8);
        InitializeStageForGameplay(GameData.selectedStage);
    }

    public void InitializeStageForGameplay(int stageIndex)
    {
        _stageServiceObj.StartStage(stageIndex);
        _currentStage = _stageServiceObj.CurrentStageIndex;
        _currentRound = _stageServiceObj.CurrentRoundIndex;
        GameManager.Instance.Get<ShopService>().GenerateShopUnits();
        PrepareTeam2UnitsForRound();
        InstantiateTeam2Units();
    }

    public void InstantiateUnit(UnitData unitData, Node node, TeamEnum team )
    {
        BaseUnit newUnit = Instantiate(unitData.unitPrefab);
        newUnit.Initialize(unitData, team, node);

        _teamServiceObj.MoveToField(newUnit, team);
        _inventoryServiceObj.RemoveUnit(newUnit.UnitData);
    }

    private void PrepareTeam2UnitsForRound()
    {
        List<RoundEnemyData> enemiesForRound = _stageServiceObj.GetCurrentRoundData().enemiyList;
        int enemyCount = enemiesForRound.Count;
        _teamServiceObj.SetFieldCapacity(TeamEnum.Team2, enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            RoundEnemyData enemy = enemiesForRound[i];

            if (_unitServiceObj.TryGetUnitById(enemy.enemyID, out UnitData enemyUnitData))
            {
                enemyUnitData.unitLevel = enemy.enemyLevel;
                _teamServiceObj.AddUnitToTeam(enemyUnitData, TeamEnum.Team2);
            }
            else
            {
                Debug.LogWarning($"Enemy ID {enemy.enemyID} not found in database.");
            }
        }
    }

    private void InstantiateTeam2Units()
    {
        int team2FieldCapacity = _teamServiceObj.GetFieldCapacity(TeamEnum.Team2);

        IReadOnlyList<UnitData> team2Units = _teamServiceObj.GetTeamUnits(TeamEnum.Team2);
        for (int i = 0; i < team2FieldCapacity; i++)
        {
            UnitData unitData = team2Units[i];
            BaseUnit newUnit = Instantiate(unitData.unitPrefab);
            newUnit.Initialize(unitData, TeamEnum.Team2, _graphServiceObj.GetUnOccupiedNode(TeamEnum.Team2));
            _teamServiceObj.MoveToField(newUnit, TeamEnum.Team2);
            _inventoryServiceObj.RemoveUnit(newUnit.UnitData);
        }
    }

    public IReadOnlyList<BaseUnit> GetOpponentTeamUnits(TeamEnum opponentTeam)
    {
        if (opponentTeam == TeamEnum.Team1)
        {
            return _teamServiceObj.GetFieldUnits(TeamEnum.Team2);
        }
        else
        {
            return _teamServiceObj.GetFieldUnits(TeamEnum.Team1);
        }
    }

    public void MarkUnitDead(BaseUnit unit)
    {
        _teamServiceObj.RemoveUnitFromField(unit, unit.Team);
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
        return _teamServiceObj.GetFieldUnitsCount(team) == 0;
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