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
        }
        else
        {
            StartStage(stageIndex);
        }
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

    public void ResumeStageFromSave(RoundSnapshotData saveData)
    {
        CurrentStageIndex = saveData.stageIndex;
        CurrentRoundIndex = saveData.roundIndex + 1;

        ApplyStageConfig();

        Debug.Log($"Resuming Stage {CurrentStageIndex}, Round {CurrentRoundIndex}");

        RestorePlayerInventory(saveData);
        RaiseStageStartedAfterRestoreEvent(saveData);

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

        foreach (UnitSnapshotData unit in saveData.playerInventoryUnits)
        {
            unitServiceObj.TryGetUnitById(unit.unitID, out UnitData unitData);
            teamServiceObj.AddUnitToTeam(unitData, TeamEnum.Team1);
        }
    }

    public bool OnRoundWin(TeamEnum winnerTeam)
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].winXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, winnerTeam, currencyReward);     
        return AdvanceRound(RoundResultEnum.Win);
    }

    public bool OnRoundLose(TeamEnum loserTeam)
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].lossXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, loserTeam, currencyReward);

        if (GameManager.Instance.Get<PlayerLevelService>().IsPlayerDead())
        {
            EventBusManager.Instance.Raise( EventNameEnum.StageFailed, CurrentStageIndex);
            return true;
        }

        return AdvanceRound(RoundResultEnum.Lose);
    }

    public bool OnRoundDraw()
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].lossXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, TeamEnum.None, currencyReward);

        return AdvanceRound(RoundResultEnum.Draw);
    }

    private bool AdvanceRound(RoundResultEnum result)
    {
        CurrentRoundIndex++;

        if (CurrentRoundIndex >= GetRoundCount())
        {
            EventBusManager.Instance.Raise(EventNameEnum.StageCleared, CurrentStageIndex);

            return true;
        }

        SaveStageProgress(result);
        StartRound();

        return false;
    }

    private void SaveStageProgress(RoundResultEnum roundResult)
    {
        RaiseRoundOverSaveSnapshotEvent();
        GameManager.Instance.Get<StageSnapshotService>().SaveStageSnapshotData();
    }

    private void RaiseRoundOverSaveSnapshotEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.RoundOverSaveSnapshot, CurrentStageIndex, CurrentRoundIndex, RoundResultEnum.InProgress);
    }

    private void RaiseStageStartedEvent()
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, CurrentStageIndex,
            stage.initialPlayerLevel, initialPlayerXP, stage.initialCurrency, stage.maxPlayerLives,
            stage.maxPlayerLives, stage.roundDataList.Count, XpExchangeCost, XpExchangeValue, ShopRefreshCost);
    }

    private void RaiseStageStartedAfterRestoreEvent(RoundSnapshotData saveData)
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, CurrentStageIndex,
            saveData.playerLevel, saveData.playerXP, saveData.playerCurrency, stage.maxPlayerLives,
            saveData.playerLives, stage.roundDataList.Count, XpExchangeCost, XpExchangeValue, ShopRefreshCost
        );
    }

    public StageData GetCurrentStageData()
    {
        return _stageConfigDataList[CurrentStageIndex];
    }

    public RoundData GetCurrentRoundData()
    {
        return GetCurrentStageData().roundDataList[CurrentRoundIndex];
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
}
