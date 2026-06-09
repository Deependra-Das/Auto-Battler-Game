using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameplayManager : MonoBehaviour
{
    [SerializeField] private float _roundEndDelay = 1.5f;


    public static GameplayManager Instance;

    protected PathFindingGraph graph;
    private TileGridService _tileGridServiceObj;
    private GraphService _graphServiceObj;
    private TeamService _teamServiceObj;
    private InventoryService _inventoryServiceObj;
    private BuffService _buffServiceObj;
    private StageService _stageServiceObj;
    private UnitDataService _unitServiceObj;
    private UnitPoolService _unitPoolServiceObj;
    private PlayerLevelService _playerLevelServiceObj;
    private ShopService _shopServiceObj;
    private RoundSnapshotService _roundSnapshotServiceObj;
    private StageSnapshotService _stageSnapshotServiceObj;
    private CurrencyService _currencyServiceObj;

    public int fromIndex = 0;
    public int toIndex = 0;

    private readonly HashSet<BaseUnit> _pendingDeadUnits = new();
    private readonly List<BaseUnit> _pendingReleaseUnits = new();

    private bool _isGameplayPaused = false;
    private bool _waitingForRoundDecision = false;
    private bool _waitingForStageDecision = false;
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
        UIManager.Instance.DestroyDiscardUnitDropZone();
        Instance = null;
    }

    public void InitializeGameplay()
    {
        ResolveServices();
        UIManager.Instance.InitializeGameplayUI();

        _tileGridServiceObj.CreateTileMap();
        _graphServiceObj.Initialize(_tileGridServiceObj.GetSpawnedTilesList());
        graph = _graphServiceObj.Graph;

        _buffServiceObj.InitializeBuffs();
        _inventoryServiceObj.SetMaxInventorySize(8);
        InitializeStageForGameplay(GameData.selectedStage);
    }

    private void ResolveServices()
    {
        _unitPoolServiceObj = GameManager.Instance.Get<UnitPoolService>();
        _tileGridServiceObj = GameManager.Instance.Get<TileGridService>();
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _buffServiceObj = GameManager.Instance.Get<BuffService>();
        _roundSnapshotServiceObj = GameManager.Instance.Get<RoundSnapshotService>();
        _stageSnapshotServiceObj = GameManager.Instance.Get<StageSnapshotService>();
        _stageServiceObj = GameManager.Instance.Get<StageService>();
        _unitServiceObj = GameManager.Instance.Get<UnitDataService>();
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
        BaseUnit newUnit = _unitPoolServiceObj.Get(card.UnitData.unitID);

        if (newUnit != null)
        {
            newUnit.gameObject.SetActive(true);
            newUnit.Initialize(card.UnitData, team, node);
            _teamServiceObj.MoveToField(newUnit, team);
            _inventoryServiceObj.RemoveUnit(card);
        }
        else
        {
            Debug.LogError($"Unit Deployement Failed for {card.UnitData.unitID}");
        }
    }

    private void PrepareTeam2UnitsForRound()
    {
        List<RoundEnemyData> enemiesForRound = _stageServiceObj.GetCurrentRoundData().enemiyList;
        int enemyCount = enemiesForRound.Count;
        _teamServiceObj.SetFieldCapacity(TeamEnum.Team2, enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            RoundEnemyData enemy = enemiesForRound[i];

            if (_unitServiceObj.TryGetUnitDataById(enemy.enemyID, out UnitData enemyUnitData))
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
            BaseUnit newUnit = _unitPoolServiceObj.Get(unitData.unitID);
            newUnit.gameObject.SetActive(true);
            newUnit.Initialize(unitData, TeamEnum.Team2, _graphServiceObj.GetUnOccupiedNode(TeamEnum.Team2));
            _teamServiceObj.MoveToField(newUnit, TeamEnum.Team2);
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

        if (_pendingDeadUnits.Contains(unit))
            return;

        _pendingDeadUnits.Add(unit);

        if (_roundCheckRoutine == null)
        {
            _roundCheckRoutine = StartCoroutine(CheckRoundEnd());
        }
    }

    private void ResolvePendingDeaths()
    {
        foreach (BaseUnit unit in _pendingDeadUnits)
        {
            if (unit == null)
                continue;

            unit.ReleaseCurrentNode();

            _teamServiceObj.RemoveUnitFromField(unit, unit.Team, false);

            _pendingReleaseUnits.Add(unit);
        }

        _pendingDeadUnits.Clear();

        foreach (BaseUnit unit in _pendingReleaseUnits)
        {
            if (unit != null)
                Destroy(unit.gameObject);
        }

        _pendingReleaseUnits.Clear();
    }

    private IEnumerator CheckRoundEnd()
    {
        yield return null;
        yield return null;


        ResolvePendingDeaths();

        bool team1HasNoUnits = TeamHasNoUnits(TeamEnum.Team1);
        bool team2HasNoUnits = TeamHasNoUnits(TeamEnum.Team2);

        TeamEnum winnerTeam = TeamEnum.None;

        if (team1HasNoUnits && team2HasNoUnits)
        {
            winnerTeam = TeamEnum.None;
        }
        else if (team1HasNoUnits)
        {
            winnerTeam = TeamEnum.Team2;
        }
        else if (team2HasNoUnits)
        {
            winnerTeam = TeamEnum.Team1;
        }
        else
        {
            _roundCheckRoutine = null;
            yield break;
        }

        yield return StartCoroutine(HandleRoundEnd(winnerTeam));

        _roundCheckRoutine = null;
    }

    private bool TeamHasNoUnits(TeamEnum team)
    {
        return _teamServiceObj.GetFieldUnitsCount(team) == 0;
    }

    private IEnumerator HandleRoundEnd(TeamEnum winnerTeam)
    {
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
            _waitingForStageDecision = true;
            yield return StartCoroutine(HandleStageFailed());
            yield break;
        }

        bool stageOver = _stageServiceObj.IsStageOver();

        if (stageOver)
        {
            _waitingForStageDecision = true;
            bool cleared = _stageServiceObj.CheckStageCleared();

            if (cleared)
            {
                yield return StartCoroutine(HandleStageClearedFull());
            }
            else
            {
                yield return StartCoroutine(HandleStageClearedPartial());
            }

            yield break;
        }

        _waitingForRoundDecision = true;
    }

    private IEnumerator HandleStageClearedFull()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);
        yield return null;
        EventBusManager.Instance.Raise(EventNameEnum.StageClearedFull, _stageServiceObj.CurrentStageIndex);
    }

    private IEnumerator HandleStageClearedPartial()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);
        yield return null;
        EventBusManager.Instance.Raise(EventNameEnum.StageClearedPartial, _stageServiceObj.CurrentStageIndex);
    }

    private IEnumerator HandleStageFailed()
    {
        UpdateGameplayState(GameplayStateEnum.StageOver);
        yield return null;
        EventBusManager.Instance.Raise(EventNameEnum.StageFailed, _stageServiceObj.CurrentStageIndex);
    }

    private void CleanupRound(bool restorePlayerInventory = true)
    {
        CleanupUnits(restorePlayerInventory);
        CleanupNodeOccupancy();
    }

    private void CleanupStage()
    {
        _isGameplayPaused = false;
        Time.timeScale = 1f;

        _pendingDeadUnits.Clear();
        _pendingReleaseUnits.Clear();

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

        List<UnitData> inventoryUnits = new(_teamServiceObj.GetInventoryUnits(TeamEnum.Team1));

        foreach (UnitData unitData in inventoryUnits)
        {
            _teamServiceObj.RemoveUnitFromInventory(unitData, TeamEnum.Team1);
        }

        _inventoryServiceObj.Reset();

        if (restorePlayerInventory)
        {
            _teamServiceObj.AddAllTeamUnitsToInventory(TeamEnum.Team1);
        }
        else
        {
            _teamServiceObj.ClearTeam(TeamEnum.Team1);
        }

        _teamServiceObj.ClearTypeCount(TeamEnum.Team1);
        _teamServiceObj.ClearFactionCount(TeamEnum.Team1);
        _buffServiceObj.RemoveAllAppliedBuffs(TeamEnum.Team1);
        UIManager.Instance.ResetBuffParticipantCountUI(TeamEnum.Team1);
    }

    private void CleanupTeam2()
    {
        List<BaseUnit> fieldUnits = new (_teamServiceObj.GetFieldUnits(TeamEnum.Team2));

        foreach (BaseUnit unit in fieldUnits)
        {
            DespawnUnit(unit);
        }

        _teamServiceObj.ClearTeam(TeamEnum.Team2);
        _teamServiceObj.ClearTypeCount(TeamEnum.Team1);
        _teamServiceObj.ClearFactionCount(TeamEnum.Team1);
        _buffServiceObj.RemoveAllAppliedBuffs(TeamEnum.Team2);
        UIManager.Instance.ResetBuffParticipantCountUI(TeamEnum.Team2);
    }

    private void DespawnUnit(BaseUnit unit)
    {
        if (unit == null)
            return;

        unit.ReleaseCurrentNode();
        _teamServiceObj.RemoveUnitFromField(unit, unit.Team, true);
        _unitPoolServiceObj.Release(unit.UnitData.unitID, unit);
    }

    private void CleanupNodeOccupancy()
    {
        _graphServiceObj.ClearGraphNodeOccupancy();
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
        if (CurrentGameplayState != GameplayStateEnum.Combat && CurrentGameplayState != GameplayStateEnum.Preparation)
        {
            return;
        }

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

    private void CommitRound()
    {
        _stageSnapshotServiceObj.SaveStageSnapshotData();
    }

    public void OnPlayerChooseNextRound()
    {
        if (!_waitingForRoundDecision) return;

        _waitingForRoundDecision = false;
        _roundCheckRoutine = null;
        CommitRound();
        CleanupRound(true);
        _stageServiceObj.TryAdvanceRound();

        PrepareCurrentRound();
    }

    public void OnPlayerChooseRestartRound()
    {
        if (!_waitingForRoundDecision && !_waitingForStageDecision) return;

        _stageServiceObj.ReduceCurrentRoundResulCounttOnRestart(_roundSnapshotServiceObj.GetRoundEndSnapshot().result);
        RestartCurrentRound();
        _stageServiceObj.RestartCurrentRound();
        PrepareCurrentRound();
    }

    public void OnRestartRoundFromPauseMenu()
    {
        if (CurrentGameplayState != GameplayStateEnum.Combat && CurrentGameplayState != GameplayStateEnum.Preparation)
        {
            return;
        }

        if (_isGameplayPaused)
        {
            RestartCurrentRound();

            _isGameplayPaused = false;
            Time.timeScale = 1f;
            EventBusManager.Instance.Raise(EventNameEnum.GameplayResumed);

            _stageServiceObj.RestartCurrentRound();
            PrepareCurrentRound();
        }
    }

    private void RestartCurrentRound()
    {
        if (_roundCheckRoutine != null)
        {
            StopCoroutine(_roundCheckRoutine);
            _roundCheckRoutine = null;
        }

        _pendingDeadUnits.Clear();
        _pendingReleaseUnits.Clear();

        _waitingForRoundDecision = false;
        _waitingForStageDecision = false;

        CleanupRound(false);
        _roundCheckRoutine = null;
    }

    public void OnPlayerLeaveStageGameplayOver()
    {
        ExitToStageSelection(true);
    }

    public void OnPlayerLeaveStageFromPauseMenu()
    {
        if (CurrentGameplayState != GameplayStateEnum.Combat && CurrentGameplayState != GameplayStateEnum.Preparation)
        {
            return;
        }

        ExitToStageSelection(false);
    }

    private void ExitToStageSelection(bool commit)
    {
        StopProcessesBeforeExit();

        if (commit)
        {
            CommitRound();
        }

        _isGameplayPaused = false;
        Time.timeScale = 1f;

        CleanupStage();

        SceneLoader.Instance.LoadScene(SceneNameEnum.StageSelectionScene);
    }

    private void StopProcessesBeforeExit()
    {
        _waitingForRoundDecision = false;
        _waitingForStageDecision = false;

        if (_roundCheckRoutine != null)
        {
            StopCoroutine(_roundCheckRoutine);
            _roundCheckRoutine = null;
        }
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
}