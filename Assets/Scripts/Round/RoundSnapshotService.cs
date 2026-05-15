using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections.Generic;

public class RoundSnapshotService
{
    private RoundSnapshotData _currentSaveState = new RoundSnapshotData();

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

    ~RoundSnapshotService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundStarted, OnRoundStartSaveSnapshot);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOverSaveSnapshot, OnRoundStartSaveSnapshot);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundStarted, OnRoundStartSaveSnapshot);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOverSaveSnapshot, OnRoundStartSaveSnapshot);
    }

    private void OnRoundStartSaveSnapshot(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];
        RoundResultEnum roundResult = (RoundResultEnum)parameters[2];

        SaveRoundSnapshotData(stageIndex, roundIndex, roundResult);
    }

    private void SaveRoundSnapshotData(int stageIndex, int roundIndex, RoundResultEnum roundResult)
    {
        _currentSaveState.stageIndex = stageIndex;
        _currentSaveState.roundIndex = roundIndex;

        _currentSaveState.playerLevel = _playerLevelServiceObj.Level;
        _currentSaveState.playerXP = _playerLevelServiceObj.CurrentXP;
        _currentSaveState.playerCurrency = _currencyServiceObj.Balance;
        _currentSaveState.playerLives = _playerLevelServiceObj.Lives;
        _currentSaveState.playerInventoryUnits = _teamServiceObj.GetTeamUnitSnapshot(TeamEnum.Team1);

        _currentSaveState.result = roundResult;
    }

    public RoundSnapshotData GetLastSavedRoundSnapshotData()
    {
        return _currentSaveState;
    }
}
