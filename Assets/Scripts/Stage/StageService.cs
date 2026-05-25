using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections.Generic;
using UnityEngine;

public class StageService
{
    private StageSnapshotService _stageSnapshotServiceObj;

    private List<StageData> _stageConfigDataList;

    public int CurrentStageIndex { get; private set; }
    public int CurrentRoundIndex { get; private set; }
    public int XpExchangeCost { get; private set; }
    public int XpExchangeValue { get; private set; }
    public int ShopRefreshCost { get; private set; }
    public int CurrentStageWinCount { get; private set; }
    public int CurrentStageDrawCount { get; private set; }
    public int CurrentStageLoseCount { get; private set; }

    private int initialPlayerXP = 0;

    public StageService(StageConfigScriptableObjectScript stageConfig)
    {
        _stageConfigDataList = stageConfig.stageConfigDataList;
        _stageSnapshotServiceObj = GameManager.Instance.Get<StageSnapshotService>();
    }

    public void InitializeStage(int stageIndex)
    {
        var saveData = _stageSnapshotServiceObj.GetStageSnapshot(stageIndex);

        if (saveData != null)
        {
            ResumeStageFromSave(saveData);
            LoadStageStats(saveData);
        }
        else
        {
            StartStage(stageIndex);
            LoadStageStats(stageIndex);
        }
    }

    private void LoadStageStats(int stageIndex)
    {
        CurrentStageWinCount = 0;
        CurrentStageDrawCount = 0;
        CurrentStageLoseCount = 0;
    }

    private void LoadStageStats(StageSnapshotEntry saveData)
    {
        CurrentStageWinCount = saveData.winCount;
        CurrentStageDrawCount = saveData.drawCount;
        CurrentStageLoseCount = saveData.loseCount;
    }

    public void StartStage(int stageIndex)
    {
        CurrentStageIndex = stageIndex;
        CurrentRoundIndex = 0;
        ApplyStageConfig();

        Debug.Log($"Starting Stage {CurrentStageIndex}");
        RaiseStageStartedEvent();

        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Starting Stage {CurrentStageIndex} - Round {CurrentRoundIndex}");
        RaiseRoundStartedEvent();
    }

    public void RestartCurrentRound()
    {
        Debug.Log($"Restarting Round {CurrentRoundIndex} of Stage {CurrentStageIndex}");
        RaiseRoundStartedEvent();
    }

    public void ResumeStageFromSave(StageSnapshotEntry saveData)
    {
        CurrentStageIndex = saveData.latestRoundSnapshot.stageIndex;
        CurrentRoundIndex = saveData.latestRoundSnapshot.roundIndex + 1;

        ApplyStageConfig();

        Debug.Log($"Resuming Stage {CurrentStageIndex}, Round {CurrentRoundIndex}");

        RestorePlayerInventory(saveData.latestRoundSnapshot);
        RaiseStageStartedAfterRestoreEvent(saveData.latestRoundSnapshot);

        StartRound();
    }

    private void ApplyStageConfig()
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        XpExchangeCost = stage.xpExchangeCost;
        XpExchangeValue = stage.xpExchangeValue;
        ShopRefreshCost = stage.shopRefreshCost;
    }

    private void RaiseRoundStartedEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.RoundStarted, CurrentStageIndex, CurrentRoundIndex, RoundResultEnum.InProgress);
    }

    private void RestorePlayerInventory(RoundSnapshotData saveData)
    {
        TeamService teamServiceObj = GameManager.Instance.Get<TeamService>();
        UnitService unitServiceObj = GameManager.Instance.Get<UnitService>();
        InventoryService inventoryServiceObj = GameManager.Instance.Get<InventoryService>();

        foreach (UnitSnapshotData unit in saveData.playerInventoryUnits)
        {
            unitServiceObj.TryGetUnitById(unit.unitID, out UnitData unitData);
            teamServiceObj.AddUnitToTeam(unitData, TeamEnum.Team1);
            inventoryServiceObj.AddUnit(unitData);
        }
    }

    public void OnRoundWin(TeamEnum winnerTeam)
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].winXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, RoundResultEnum.Win, winnerTeam, currencyReward);

        SaveStageProgress(RoundResultEnum.Win);
    }

    public void OnRoundLose(TeamEnum loserTeam)
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].lossXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, RoundResultEnum.Lose, loserTeam, currencyReward);

        SaveStageProgress(RoundResultEnum.Lose);
    }

    public void OnRoundDraw()
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].lossXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, RoundResultEnum.Draw, TeamEnum.None, currencyReward);

        SaveStageProgress(RoundResultEnum.Draw);
    }

    public bool CheckStageCleared()
    {
        return (CurrentStageWinCount + CurrentStageDrawCount) == GetRoundCount();
    }

    public bool TryAdvanceRound()
    {
        int nextIndex = CurrentRoundIndex + 1;

        if (nextIndex >= GetRoundCount())
        {
            EventBusManager.Instance.Raise(EventNameEnum.StageOver, CurrentStageIndex);
            return true;
        }

        CurrentRoundIndex = nextIndex;
        return false;
    }

    private void SaveStageProgress(RoundResultEnum roundResult)
    {
        RaiseRoundOverSaveSnapshotEvent(roundResult);
        GameManager.Instance.Get<StageSnapshotService>().SaveStageSnapshotData();
    }

    private void RaiseRoundOverSaveSnapshotEvent(RoundResultEnum roundResult)
    {
        EventBusManager.Instance.Raise(EventNameEnum.RoundOverSaveSnapshot, CurrentStageIndex, CurrentRoundIndex, roundResult);
    }

    private void RaiseStageStartedEvent()
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, 
            CurrentStageIndex, CurrentRoundIndex, stage.roundDataList.Count,
            stage.initialPlayerLevel, initialPlayerXP, stage.maxPlayerLives,
            XpExchangeCost, XpExchangeValue, ShopRefreshCost, stage.initialCurrency);
    }

    private void RaiseStageStartedAfterRestoreEvent(RoundSnapshotData saveData)
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, 
            CurrentStageIndex, CurrentRoundIndex, stage.roundDataList.Count,
            saveData.playerLevel, saveData.playerXP, saveData.playerLives, 
            XpExchangeCost, XpExchangeValue, ShopRefreshCost, saveData.playerCurrency
        );
    }

    public StageData GetCurrentStageData()
    {
        return _stageConfigDataList[CurrentStageIndex];
    }

    public RoundData GetCurrentRoundData()
    {
        var stage = GetCurrentStageData();

        if (CurrentRoundIndex < 0 || CurrentRoundIndex >= stage.roundDataList.Count)
        {
            return null;
        }

        return stage.roundDataList[CurrentRoundIndex];
    }

    public int GetRoundCount()
    {
        return _stageConfigDataList[CurrentStageIndex].roundDataList.Count;
    }

    public int GetStageCount()
    {
        return _stageConfigDataList.Count;
    }

    public StageData GetStageData(int index)
    {
        return _stageConfigDataList[index];
    }

    public void Reset()
    {
        CurrentStageIndex = 0;
        CurrentRoundIndex = 0;

        XpExchangeCost = 0;
        XpExchangeValue = 0;
        ShopRefreshCost = 0;

        CurrentStageWinCount = 0;
        CurrentStageDrawCount = 0;
        CurrentStageLoseCount = 0;

        initialPlayerXP = 0;
    }

    public void Dispose()
    {
        Reset();

        _stageConfigDataList = null;
        _stageSnapshotServiceObj = null;
    }
}
