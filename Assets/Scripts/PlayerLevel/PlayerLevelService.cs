using UnityEngine;

public class PlayerLevelService
{
    private CurrencyService _currencyService;
    private PlayerLevelConfigScriptableObjectScript _config;

    public int Level { get; private set; } = 1;
    public int Lives { get; private set; } = 3;

    public PlayerLevelService(CurrencyService currencyService, PlayerLevelConfigScriptableObjectScript config)
    {
        _currencyService = currencyService;
        _config = config;
    }

    public int MaxLevel => _config.playerProgressionDataList.Count;
    public int MaxUnits => _config.playerProgressionDataList[MaxLevel-1].maxUnitsAllowed;

    public PlayerLevelData GetCurrentPlayerLevelData()
    {
        int index = Mathf.Clamp(Level - 1, 0, MaxLevel - 1);
        return _config.playerProgressionDataList[index];
    }

    public bool CanLevelUp()
    {
        if (Level >= MaxLevel)
            return false;

        int cost = GetNextPlayerLevelCost();
        return _currencyService.CanAfford(cost);
    }

    public bool TryLevelUp()
    {
        if (Level >= MaxLevel)
            return false;

        int cost = GetNextPlayerLevelCost();

        if (!_currencyService.SpendCurrency(cost))
            return false;

        Level++;
        return true;
    }

    public int GetNextPlayerLevelCost()
    {
        if (Level >= MaxLevel)
            return int.MaxValue;

        return _config.playerProgressionDataList[Level].xpCurrencyRequired;
    }

    public void LoseLife()
    {
        Lives = Mathf.Max(0, Lives - 1);
    }

    public bool IsPlayerDead()
    {
        return Lives <= 0;
    }

    public void Reset()
    {
        Level = 1;
        Lives = 3;
    }
}