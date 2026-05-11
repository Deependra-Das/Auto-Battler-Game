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
        int initialPlayerLevel = _stageConfigDataList[CurrentStageIndex].initialPlayerLevel;
        int initialCurrency = _stageConfigDataList[CurrentStageIndex].initialCurrency;
        int maxLives = _stageConfigDataList[CurrentStageIndex].maxPlayerLives;
        int roundCount = _stageConfigDataList[CurrentStageIndex].roundDataList.Count;
        XpExchangeCost = _stageConfigDataList[CurrentStageIndex].xpExchangeCost;
        XpExchangeValue = _stageConfigDataList[CurrentStageIndex].xpExchangeValue;
        ShopRefreshCost = _stageConfigDataList[CurrentStageIndex].shopRefreshCost;
        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, CurrentStageIndex, initialPlayerLevel, initialCurrency, maxLives, roundCount, XpExchangeCost, XpExchangeValue, ShopRefreshCost);
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Starting Stage {CurrentStageIndex} - Round {CurrentRoundIndex}");
        EventBusManager.Instance.Raise(EventNameEnum.RoundStarted,CurrentRoundIndex, CurrentStageIndex);
    }

    public bool OnRoundWin(TeamEnum winnerTeam)
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].winXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, winnerTeam, currencyReward);     
        return AdvanceRound();
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

        return AdvanceRound();
    }

    public bool OnRoundDraw()
    {
        int currencyReward = _stageConfigDataList[CurrentStageIndex].roundDataList[CurrentRoundIndex].lossXPCurrency;
        EventBusManager.Instance.Raise(EventNameEnum.RoundOver, TeamEnum.None, currencyReward);

        return AdvanceRound();
    }

    private bool AdvanceRound()
    {
        CurrentRoundIndex++;

        if (CurrentRoundIndex >= GetRoundCount())
        {
            EventBusManager.Instance.Raise(EventNameEnum.StageCleared, CurrentStageIndex);

            return true;
        }

        StartRound();

        return false;
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
