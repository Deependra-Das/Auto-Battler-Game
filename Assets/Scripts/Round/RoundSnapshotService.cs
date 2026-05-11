using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections.Generic;

public class RoundSnapshotService
{
    private RoundSnapshotData _currentSaveState;

    private PlayerLevelService _playerLevelServiceObj;
    private CurrencyService _currencyServiceObj;
    private InventoryService _inventoryServiceObj;

    public RoundSnapshotService()
    {
        _playerLevelServiceObj = GameManager.Instance.Get<PlayerLevelService>();
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        SubscribeToEvents();
    }

    ~RoundSnapshotService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundStarted, OnRoundStartSaveState);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundStarted, OnRoundStartSaveState);
    }

    private void OnRoundStartSaveState(object[] parameters)
    {
        int roundIndex = (int)parameters[0];
        int stageIndex = (int)parameters[1];

        SaveRoundStateData(stageIndex, roundIndex);
    }

    private void SaveRoundStateData(int stageIndex, int roundIndex)
    {
        _currentSaveState = new RoundSnapshotData
        {
            stageIndex = stageIndex,
            roundIndex = roundIndex,

            playerLevel = _playerLevelServiceObj.Level,
            playerXP = _playerLevelServiceObj.CurrentXP,
            playerCurrency = _currencyServiceObj.Balance,

            playerInventoryUnits = GetInventorySaveData(_inventoryServiceObj.GetInventoryUnits()),

            result = RoundResultEnum.InProgress
        };
    }

    private List<UnitSaveData> GetInventorySaveData(IReadOnlyList<UnitData> unitData)
    {
        List<UnitSaveData> unitSaveData = new();

        List<UnitData> inventoryUnits = _inventoryServiceObj.GetInventoryUnits();

        foreach (var unit in inventoryUnits)
        {
            unitSaveData.Add(new UnitSaveData
            {
                unitID = unit.unitID,
                unitLevel = unit.unitLevel
            });
        }

        return unitSaveData;
    }
}
