using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private float _roundEndCheckDelay = 1f;
    [SerializeField] private float _roundEndDelay = 1.5f;
    [SerializeField] private float _stageTransitionDelay = 0.5f;
    [SerializeField] private float _stageResultDelay = 2f;
    [SerializeField] private float _sceneChangeDelay = 5f;

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
    private RoundSnapshotService _roundSnapshotServiceObj;
    private CurrencyService _currencyServiceObj;

    public int fromIndex = 0;
    public int toIndex = 0;

    private bool _isRoundEnding = false;
    private bool _isGameplayPaused = false;

    private Coroutine _roundCheckRoutine;

    public GameplayStateEnum CurrentGameplayState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeGameplay();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        CleanupStage();

        StopAllCoroutines();

        Instance = null;
    }

    public void InitializeGameplay()
    {
        ResolveServices();
        UIManager.Instance.InitializeGameplayUI();
        InventoryDropZoneManager.Instance.Initialize();

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
        _roundSnapshotServiceObj = GameManager.Instance.Get<RoundSnapshotService>();
        _stageServiceObj = GameManager.Instance.Get<StageService>();
        _unitServiceObj = GameManager.Instance.Get<UnitService>();
        _playerLevelServiceObj = GameManager.Instance.Get<PlayerLevelService>();
        _shopServiceObj = GameManager.Instance.Get<ShopService>();
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
    }

    public void InitializeStageForGameplay(int stageIndex)
    {
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

        foreach (UnitData unitData in _teamServiceObj.GetTeamUnits(TeamEnum.Team2))
        {
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
        if (unit == null) return;

        unit.ReleaseCurrentNode();
        _teamServiceObj.RemoveUnitFromField(unit, unit.Team);
        Destroy(unit.gameObject);

        if (_roundCheckRoutine != null) return;

        _roundCheckRoutine = StartCoroutine(CheckRoundEnd());
    }

    private IEnumerator CheckRoundEnd()
    {
        if (_isRoundEnding)
            yield break;

        _isRoundEnding = true;

        yield return new WaitForSeconds(_roundEndCheckDelay);

        bool team1HasNoUnits = TeamHasNoUnits(TeamEnum.Team1);
        bool team2HasNoUnits = TeamHasNoUnits(TeamEnum.Team2);

        if (!team1HasNoUnits && !team2HasNoUnits)
        {
            _isRoundEnding = false;
            _roundCheckRoutine = null;
            yield break;
        }

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
        _isRoundEnding = true;

        UpdateGameplayState(GameplayStateEnum.RoundOver);

        yield return new WaitForSeconds(_roundEndDelay);

        bool stageFailed = false;

        switch (winnerTeam)
        {
            case TeamEnum.Team1:
                _stageServiceObj.OnRoundWin(TeamEnum.Team1);
                break;

            case TeamEnum.Team2:
                _stageServiceObj.OnRoundLose(TeamEnum.Team1);

                if (_playerLevelServiceObj.IsPlayerDead())
                {
                    stageFailed = true;
                }

                break;

            case TeamEnum.None:
                _stageServiceObj.OnRoundDraw();
                break;
        }

        UpdateGameplayState(GameplayStateEnum.Cleanup);

        if (stageFailed)
        {
            CleanupStage();
            yield return StartCoroutine(HandleStageFailed());

            _isRoundEnding = false;
            yield break;
        }

        bool stageOver = _stageServiceObj.TryAdvanceRound();

        if (stageOver)
        {
            CleanupStage();

            bool cleared = _stageServiceObj.CheckStageCleared();

            if (cleared)
            {
                yield return StartCoroutine(HandleStageClearedFull());
            }
            else
            {
                yield return StartCoroutine(HandleStageClearedPartial());
            }

            _isRoundEnding = false;
            yield break;
        }

        CleanupRound(true);

        yield return new WaitForSeconds(_stageTransitionDelay);
        PrepareCurrentRound();
        _isRoundEnding = false;
    }

    private IEnumerator HandleStageClearedFull()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);

        Debug.Log("Stage Cleared Full");

        yield return new WaitForSeconds(_stageResultDelay);
    }

    private IEnumerator HandleStageClearedPartial()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);

        Debug.Log("Stage Cleared Partial");

        yield return new WaitForSeconds(_stageResultDelay);
    }

    private IEnumerator HandleStageFailed()
    {
        EventBusManager.Instance.Raise(EventNameEnum.StageFailed, _stageServiceObj.CurrentStageIndex);

        UpdateGameplayState(GameplayStateEnum.StageOver);

        Debug.Log("Stage Failed");

        yield return new WaitForSeconds(_stageResultDelay);
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

    private void CleanupRound(bool restorePlayerInventory = true)
    {
        CleanupUnits(restorePlayerInventory);
    }

    private void CleanupStage()
    {
        _isRoundEnding = false;
        _isGameplayPaused = false;
        Time.timeScale = 1f;

        CleanupRound(false);

        _tileGridServiceObj.Reset();
        _graphServiceObj.Reset();
        _roundSnapshotServiceObj.Reset();
        _stageServiceObj.Reset();
        _inventoryServiceObj.Reset();
        _teamServiceObj.Reset();
        _buffServiceObj.Reset();
        _playerLevelServiceObj.Reset();
        _shopServiceObj.Reset();
        _currencyServiceObj.Reset();

        UpdateGameplayState(GameplayStateEnum.Preparation);
    }

    private void CleanupUnits(bool restorePlayerInventory)
    {
        CleanupTeam1(restorePlayerInventory);
        CleanupTeam2();
    }

    private void CleanupTeam1(bool restorePlayerInventory)
    {
        List<BaseUnit> fieldUnits = new (_teamServiceObj.GetFieldUnits(TeamEnum.Team1));

        foreach (BaseUnit unit in fieldUnits)
        {
            DespawnUnit(unit);
        }

        if (restorePlayerInventory)
        {
            _teamServiceObj.AddAllTeamUnitsToInventory(TeamEnum.Team1);
        }
        else
        {
            _teamServiceObj.ClearTeam(TeamEnum.Team1);
        }
    }

    private void CleanupTeam2()
    {
        List<BaseUnit> fieldUnits = new (_teamServiceObj.GetFieldUnits(TeamEnum.Team2));

        foreach (BaseUnit unit in fieldUnits)
        {
            DespawnUnit(unit);
        }

        _teamServiceObj.ClearTeam(TeamEnum.Team2);
    }

    private void DespawnUnit(BaseUnit unit)
    {
        if (unit == null)
            return;

        unit.ReleaseCurrentNode();
        _teamServiceObj.RemoveUnitFromField(unit, unit.Team);
        Destroy(unit.gameObject);    
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

    public void TogglePause()
    {
        if (_isGameplayPaused)
        {
            ResumeGameplay();
        }
        else
        {
            PauseGameplay();
        }
    }
    public void PauseGameplay()
    {
        if (_isGameplayPaused) return;

        _isGameplayPaused = true;

        Time.timeScale = 0f;

        EventBusManager.Instance.Raise(EventNameEnum.GameplayPaused);
    }

    public void ResumeGameplay()
    {
        if (!_isGameplayPaused)
            return;

        _isGameplayPaused = false;

        Time.timeScale = 1f;

        EventBusManager.Instance.Raise(EventNameEnum.GameplayResumed);
    }
}