using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;

public class RoundSaveStateService
{
    private RoundSaveStateData currentSaveState;

    public RoundSaveStateData CurrentSaveState => currentSaveState;

    public void StartRound(int stageIndex, int roundIndex)
    {
        CaptureSaveState(stageIndex, roundIndex);

        EventBusManager.Instance.Raise(
            EventNameEnum.RoundStarted,
            roundIndex
        );
    }

    public void CaptureSaveState(int stageIndex, int roundIndex)
    {
        currentSaveState = new RoundSaveStateData
        {
            stageIndex = stageIndex,
            roundIndex = roundIndex,

            playerLevel = GameManager.Instance.Get<PlayerLevelService>().Level,
            playerXP = GameManager.Instance.Get<PlayerLevelService>().CurrentXP,
            playerCurrency = GameManager.Instance.Get<CurrencyService>().Balance,

            playerInventoryUnits = GameManager.Instance.Get<InventoryService>().GetInventoryUnits(),

            result = RoundResultEnum.InProgress
        };
    }

    public void SetRoundResult(RoundResultEnum result)
    {
        currentSaveState.result = result;

        EventBusManager.Instance.Raise(
            EventNameEnum.RoundEnded,
            result
        );
    }

    public void RestartRound()
    {
        if (currentSaveState == null)
            return;

        var playerLevel = GameManager.Instance.Get<PlayerLevelService>();
        var inventory = GameManager.Instance.Get<InventoryService>();
        var currency = GameManager.Instance.Get<CurrencyService>();

        playerLevel.SetLevel(currentSaveState.playerLevel);
        currency.SetCurrency(currentSaveState.playerCurrency);

        inventory.Restore(currentSaveState.playerInventoryUnits);

        StartRound(currentSaveState.stageIndex, currentSaveState.roundIndex);
    }
}