using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;

public class RoundSnapshotService
{
    private RoundSnapshotData _roundStartSnapshot;
    private RoundSnapshotData _roundEndSnapshot;

    private PlayerLevelService _playerLevelServiceObj;
    private CurrencyService _currencyServiceObj;
    private TeamService _teamServiceObj;

    public RoundSnapshotService()
    {
        _playerLevelServiceObj = GameManager.Instance.Get<PlayerLevelService>();
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundStarted, OnRoundStartSaveSnapshot);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOver, OnRoundOverSaveSnapshot);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundStarted, OnRoundStartSaveSnapshot);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOver, OnRoundOverSaveSnapshot);
    }

    private void OnRoundStartSaveSnapshot(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];
        RoundResultEnum roundResult = (RoundResultEnum)parameters[2];

        _roundStartSnapshot = CreateSnapshot(stageIndex, roundIndex, roundResult);
        PrintRoundSnapshotData(_roundStartSnapshot);
    }

    private void OnRoundOverSaveSnapshot(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];
        RoundResultEnum roundResult = (RoundResultEnum)parameters[2];

        _roundEndSnapshot = CreateSnapshot(stageIndex, roundIndex, roundResult);
        PrintRoundSnapshotData(_roundEndSnapshot);
    }

    private RoundSnapshotData CreateSnapshot(int stageIndex, int roundIndex, RoundResultEnum roundResult)
    {
        return new RoundSnapshotData
        {
            stageIndex = stageIndex,
            roundIndex = roundIndex,

            playerLevel = _playerLevelServiceObj.Level,
            playerXP = _playerLevelServiceObj.CurrentXP,
            playerCurrency = _currencyServiceObj.Balance,
            playerLives = _playerLevelServiceObj.Lives,

            playerInventoryUnits = _teamServiceObj.GetTeamUnitSnapshot(TeamEnum.Team1),

            result = roundResult
        };
    }

    public RoundSnapshotData GetRoundStartSnapshot()
    {
        return _roundStartSnapshot;
    }

    public RoundSnapshotData GetRoundEndSnapshot()
    {
        return _roundEndSnapshot;
    }

    private void PrintRoundSnapshotData(RoundSnapshotData roundSnapshotData)
    {
        Debug.Log(
            $"===== ROUND SNAPSHOT =====\n" +
            $"Stage Index      : {roundSnapshotData.stageIndex}\n" +
            $"Round Index      : {roundSnapshotData.roundIndex}\n" +
            $"Player Level     : {roundSnapshotData.playerLevel}\n" +
            $"Player XP        : {roundSnapshotData.playerXP}\n" +
            $"Player Currency  : {roundSnapshotData.playerCurrency}\n" +
            $"Player Lives     : {roundSnapshotData.playerLives}\n" +
            $"Round Result     : {roundSnapshotData.result}\n" +
            $"Inventory Count  : {(roundSnapshotData.playerInventoryUnits != null ? roundSnapshotData.playerInventoryUnits.Count : 0)}"
        );
    }

    public void Reset()
    {
        _roundStartSnapshot = new RoundSnapshotData();
        _roundEndSnapshot = new RoundSnapshotData();
    }

    public void Dispose()
    {
        UnsubscribeToEvents();

        Reset();

        _playerLevelServiceObj = null;
        _currencyServiceObj = null;
        _teamServiceObj = null;
    }
}
