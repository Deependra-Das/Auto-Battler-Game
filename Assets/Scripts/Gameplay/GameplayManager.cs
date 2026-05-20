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
    private PlayerLevelService _playerLevelServiceObj;
    private ShopService _shopServiceObj;
    private StageSnapshotService _stageSnapshotServiceObj;

    public int fromIndex = 0;
    public int toIndex = 0;

    private bool _isRoundEnding = false;

    public GameplayStateEnum CurrentGameplayState { get; private set; }

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

        ResolveServices();

        _tileGridServiceObj.CreateTileMap();
        _graphServiceObj.Initialize(_tileGridServiceObj.GetSpawnedTilesList());
        graph = _graphServiceObj.Graph;

        _buffServiceObj.InitializeBuffs();
        _inventoryServiceObj.SetMaxInventorySize(8);
        InitializeStageForGameplay(GameData.selectedStage);
    }

    private void ResolveServices()
    {
        _tileGridServiceObj = GameManager.Instance.Get<TileGridService>();
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _buffServiceObj = GameManager.Instance.Get<BuffService>();
        _stageSnapshotServiceObj = GameManager.Instance.Get<StageSnapshotService>();
        _stageServiceObj = GameManager.Instance.Get<StageService>();
        _unitServiceObj = GameManager.Instance.Get<UnitService>();
        _playerLevelServiceObj = GameManager.Instance.Get<PlayerLevelService>();
        _shopServiceObj = GameManager.Instance.Get<ShopService>();
    }

    public void InitializeStageForGameplay(int stageIndex)
    {
        CleanupStage();
        _stageServiceObj.InitializeStage(stageIndex);
        PrepareCurrentRound();
    }

    private void PrepareCurrentRound()
    {
        UpdateGameplayState(GameplayStateEnum.Preparation);
        _shopServiceObj.GenerateShopUnits();
        PrepareTeam2UnitsForRound();
        InstantiateTeam2Units();
    }

    public void DeployUnit(InventoryUnitCard card, Node node, TeamEnum team)
    {
        BaseUnit newUnit = Instantiate(card.UnitData.unitPrefab);
        newUnit.Initialize(card.UnitData, team, node);

        _teamServiceObj.MoveToField(newUnit, team);
        _inventoryServiceObj.RemoveUnit(newUnit.UnitData);
        UIManager.Instance.RemoveInventoryUnitCard(card);
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
        if (_isRoundEnding)
            yield break;

        yield return new WaitForSeconds(1f);
        bool team1HasNoUnits = TeamHasNoUnits(TeamEnum.Team1);
        bool team2HasNoUnits = TeamHasNoUnits(TeamEnum.Team2);

        if (!team1HasNoUnits && !team2HasNoUnits)
            yield break;

        _isRoundEnding = true;

        TeamEnum winnerTeam = TeamEnum.None;

        if (team1HasNoUnits && !team2HasNoUnits)
        {
            winnerTeam = TeamEnum.Team2;
        }
        else if (team2HasNoUnits && !team1HasNoUnits)
        {
            winnerTeam = TeamEnum.Team1;
        }
        else if (team1HasNoUnits && team2HasNoUnits)
        {
            winnerTeam = TeamEnum.None;
        }
        else
        {
            yield break;
        }

        yield return StartCoroutine(HandleRoundEnd(winnerTeam));
    }

    private bool TeamHasNoUnits(TeamEnum team)
    {
        return _teamServiceObj.GetFieldUnitsCount(team) == 0;
    }

    private IEnumerator HandleRoundEnd(TeamEnum winnerTeam)
    {
        UpdateGameplayState(GameplayStateEnum.RoundOver);

        yield return new WaitForSeconds(1.5f);

        UpdateGameplayState(GameplayStateEnum.Cleanup);

        CleanupRound();

        yield return new WaitForSeconds(0.5f);

        switch (winnerTeam)
        {
            case TeamEnum.Team1:
            {
                bool roundCleared = _stageServiceObj.OnRoundWin(TeamEnum.Team1);

                if (roundCleared)
                {
                    yield return StartCoroutine(HandleStageCleared());
                }
                else
                {
                    PrepareCurrentRound();
                }

                break;
            }

            case TeamEnum.Team2:
            {
                bool stageFailed = _stageServiceObj.OnRoundLose(TeamEnum.Team1);

                if (stageFailed)
                {
                    yield return StartCoroutine(HandleStageFailed());
                }
                else
                {
                    PrepareCurrentRound();
                }

                break;
            }

            case TeamEnum.None:
            {
                bool stageAdvanced = _stageServiceObj.OnRoundDraw();

                if (!stageAdvanced)
                {
                    PrepareCurrentRound();
                }
                break;
            }
        }

        _isRoundEnding = false;
    }

    private IEnumerator HandleStageCleared()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);

        Debug.Log("Stage Cleared");

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator HandleStageFailed()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);

        Debug.Log("Stage Failed");

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator RestartRoundRoutine()
    {
        //UpdateGameplayState(GameplayStateEnum.Cleanup);

        //CleanupRound();

        yield return new WaitForSeconds(0.5f);

        //_roundSaveStateService.RestoreSaveState();

        //PrepareCurrentRound();

        //UpdateGameplayState(GameplayStateEnum.Preparation);
    }

    private void CleanupRound()
    {
        CleanupUnits();
        CleanupRuntimeGameplayData();
    }

    private void CleanupStage()
    {
        CleanupRound();
    }

    private void CleanupUnits()
    {
        CleanupTeam1();
        CleanupTeam2();
    }

    private void CleanupTeam1()
    {
        List<BaseUnit> fieldUnits = new (_teamServiceObj.GetFieldUnits(TeamEnum.Team1));

        foreach (BaseUnit unit in fieldUnits)
        {
            if (unit == null)
                continue;

            unit.ReleaseCurrentNode();
            _teamServiceObj.RemoveUnitFromField(unit, TeamEnum.Team1);
            Destroy(unit.gameObject);
        }

        _teamServiceObj.AddAllTeamUnitsToInventory(TeamEnum.Team1);
    }

    private void CleanupTeam2()
    {
        List<BaseUnit> fieldUnits = new (_teamServiceObj.GetFieldUnits(TeamEnum.Team2));

        foreach (BaseUnit unit in fieldUnits)
        {
            if (unit == null)
                continue;

            unit.ReleaseCurrentNode();
            _teamServiceObj.RemoveUnitFromField(unit, TeamEnum.Team2);
            Destroy(unit.gameObject);
        }

        List<UnitData> teamUnits = new(_teamServiceObj.GetTeamUnits(TeamEnum.Team2));

        foreach (UnitData unitData in teamUnits)
        {
            _teamServiceObj.RemoveUnitFromTeam(unitData, TeamEnum.Team2);
        }
    }

    private void CleanupRuntimeGameplayData()
    {
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