using UnityEngine;

public class PlayerLevelService
{
    private CurrencyService _currencyService;
    private PlayerLevelConfigScriptableObjectScript _config;

    public int Level { get; private set; } = 1;
    public int Lives { get; private set; } = 3;
    public int CurrentXP { get; private set; } = 0;

    public PlayerLevelService(CurrencyService currencyService, PlayerLevelConfigScriptableObjectScript config)
    {
        _currencyService = currencyService;
        _config = config;
    }

    public int MaxLevel => _config.playerProgressionDataList.Count;

    public int MaxUnits => GetCurrentPlayerLevelData().maxUnitsAllowed;

    public PlayerLevelData GetCurrentPlayerLevelData()
    {
        int index = Level - 1;
        return _config.playerProgressionDataList[index];
    }

    public int GetXPToNextLevel()
    {
        if (Level >= MaxLevel)
            return 0;

        int index = Level - 1;
        return _config.playerProgressionDataList[index].xpRequiredToNextLevel;
    }

    public bool CanBuyXP(int amount)
    {
        return _currencyService.CanAfford(amount);
    }

    public bool BuyXP(int coinsAmount)
    {
        if (Level >= MaxLevel)
            return false;

        if (!_currencyService.SpendCurrency(coinsAmount))
            return false;

        int xpGained = coinsAmount * _config.xpPerCoin;
        CurrentXP += xpGained;

        HandleLevelUp();
        return true;
    }

    private void HandleLevelUp()
    {
        while (Level < MaxLevel)
        {
            int xpRequired = GetXPToNextLevel();

            if (CurrentXP < xpRequired)
                break;

            CurrentXP -= xpRequired;
            Level++;
        }

        if (Level >= MaxLevel)
        {
            Level = MaxLevel;
            CurrentXP = 0;
        }
    }

    public float GetXPProgressNormalized()
    {
        if (Level >= MaxLevel)
            return 1f;

        return (float)CurrentXP / GetXPToNextLevel();
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
        CurrentXP = 0;
    }
}