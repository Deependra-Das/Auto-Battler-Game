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
        EventBusManager.Instance.Raise(EventNameEnum.RoundStarted,CurrentRoundIndex, CurrentStageIndex);
    }

    public void RestartCurrentRound()
    {
        Debug.Log($"Restarting Round {CurrentRoundIndex} of Stage {CurrentStageIndex}");

        EventBusManager.Instance.Raise(EventNameEnum.RoundStarted, CurrentRoundIndex, CurrentStageIndex);
    }

    public void ResumeStageFromSave(RoundSnapshotData saveData)
    {
        CurrentStageIndex = saveData.stageIndex;
        CurrentRoundIndex = saveData.roundIndex + 1;

        ApplyStageConfig();

        Debug.Log($"Resuming Stage {CurrentStageIndex}, Round {CurrentRoundIndex}");

        RaiseStageStartedEvent();

        RestorePlayerState(saveData);

        StartRound();
    }

    private void ApplyStageConfig()
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        XpExchangeCost = stage.xpExchangeCost;
        XpExchangeValue = stage.xpExchangeValue;
        ShopRefreshCost = stage.shopRefreshCost;
    }

    private void RaiseStageStartedEvent()
    {
        var stage = _stageConfigDataList[CurrentStageIndex];

        EventBusManager.Instance.Raise(EventNameEnum.StageStarted, CurrentStageIndex,
            stage.initialPlayerLevel, stage.initialCurrency,stage.maxPlayerLives,
            stage.roundDataList.Count, XpExchangeCost, XpExchangeValue,ShopRefreshCost
        );
    }

    private void RestorePlayerState(RoundSnapshotData saveData)
    {
        //var playerLevel = GameManager.Instance.Get<PlayerLevelService>();
        //var currency = GameManager.Instance.Get<CurrencyService>();
        //var inventory = GameManager.Instance.Get<InventoryService>();

        //playerLevel.SetLevel(saveData.playerLevel);
        //playerLevel.SetXP(saveData.playerXP);

        //currency.(saveData.playerCurrency);

        //inventory.ClearInventory();

        //foreach (var unit in saveData.playerInventoryUnits)
        //{
        //    inventory.AddUnit(unit);
        //}
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
        PlayerLevelService playerLevelObj = GameManager.Instance.Get<PlayerLevelService>();
        CurrencyService currencyObj = GameManager.Instance.Get<CurrencyService>();
        InventoryService inventoryObj = GameManager.Instance.Get<InventoryService>();

        RoundSnapshotData data = new RoundSnapshotData
        {
            stageIndex = CurrentStageIndex,
            roundIndex = CurrentRoundIndex,
            playerLevel = playerLevelObj.Level,
            playerXP = playerLevelObj.CurrentXP,
            playerCurrency = currencyObj.Balance,

            playerInventoryUnits = inventoryObj.GetInventoryUnitsSnapshot(),

            result = roundResult
        };

        _stageSnapshotServiceObj.Save(data);
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
