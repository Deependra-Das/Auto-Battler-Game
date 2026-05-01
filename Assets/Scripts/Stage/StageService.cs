using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections.Generic;
using UnityEngine;

public class StageService
{
    private List<StageData> _stageConfigDataList;

    public int CurrentStageIndex { get; private set; }
    public int CurrentRoundIndex { get; private set; }
    public int XpExchangeCost { get; private set; }
    public int XpExchangeValue { get; private set; }
    public int ShopRefreshCost { get; private set; }

    public StageService(StageConfigScriptableObjectScript stageConfig)
    {
        _stageConfigDataList = stageConfig.stageConfigDataList;
    }

    public void StartStage(int stageIndex)
    {
        CurrentStageIndex = stageIndex;
        CurrentRoundIndex = 0;
        Debug.Log($"Starting Stage {CurrentStageIndex}");
        int initialiPlayerLevel = _stageConfigDataList[CurrentStageIndex].initialPlayerLevel;
        int initialCurrency = _stageConfigDataList[CurrentStageIndex].initialCurrency;
        int maxLives = _stageConfigDataList[CurrentStageIndex].maxPlayerLives;
        int roundCount = _stageConfigDataList[CurrentStageIndex].roundDataList.Count;
        XpExchangeCost = _stageConfigDataList[CurrentStageIndex].xpExchangeCost;
        XpExchangeValue = _stageConfigDataList[CurrentStageIndex].xpExchangeValue;
        ShopRefreshCost = _stageConfigDataList[CurrentStageIndex].shopRefreshCost;
        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, CurrentStageIndex, initialiPlayerLevel, initialCurrency, maxLives, roundCount, XpExchangeCost, XpExchangeValue, ShopRefreshCost);
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Starting Stage {CurrentStageIndex} - Round {CurrentRoundIndex}");

        //GameManager.Instance.Get<RoundService>().CaptureRoundSaveState(CurrentStageIndex, CurrentRoundIndex);
        EventBusManager.Instance.Raise(EventNameEnum.RoundStarted,CurrentRoundIndex);
    }

    public void OnRoundWin()
    {
        var round = GetCurrentRoundData();
        AdvanceRound();
    }

    public void OnRoundLose()
    {
        var round = GetCurrentRoundData();

        if (GameManager.Instance.Get<PlayerLevelService>().IsPlayerDead())
        {
            OnStageFailed();
            return;
        }

        AdvanceRound();
    }

    private void AdvanceRound()
    {
        CurrentRoundIndex++;

        if (CurrentRoundIndex >= GetRoundCount())
        {
            OnStageCleared();
        }
        else
        {
            StartRound();
        }
    }

    private void OnStageCleared()
    {
        Debug.Log("Stage Cleared!");

        if (CurrentStageIndex >= _stageConfigDataList.Count - 1)
        {
            OnGameCompleted();
            return;
        }

        StartStage(CurrentStageIndex + 1);
    }

    private void OnGameCompleted()
    {
        Debug.Log("Game Completed!");
    }

    private void OnStageFailed()
    {
        Debug.Log("Stage Failed. Restarting...");
        StartStage(CurrentStageIndex);
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
