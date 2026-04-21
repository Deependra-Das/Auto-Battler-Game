using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;

public class PlayerLevelService
{
    private CurrencyService _currencyService;
    private PlayerLevelConfigScriptableObjectScript _config;

    public int Level { get; private set; } = 1;
    public int Lives { get; private set; } = 3;
    public int CurrentXP { get; private set; } = 0;
    public int XPPerCoin { get; private set; } = 0;

    public PlayerLevelService(PlayerLevelConfigScriptableObjectScript config)
    {
        SubscribeToEvents();
        _currencyService = GameManager.Instance.Get<CurrencyService>();
        _config = config;
        XPPerCoin = _config.xpPerCoin;
    }

    ~PlayerLevelService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
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

        EventBusManager.Instance.Raise(EventNameEnum.XPChanged, GetXPProgressNormalized());

        HandleLevelUp();
        return true;
    }

    private void HandleLevelUp()
    {
        bool leveledUp = false;

        while (Level < MaxLevel)
        {
            int xpRequired = GetXPToNextLevel();

            if (CurrentXP < xpRequired)
                break;

            CurrentXP -= xpRequired;
            Level++;
            leveledUp = true;
        }

        if (Level >= MaxLevel)
        {
            Level = MaxLevel;
            CurrentXP = 0;
        }
        if (leveledUp)
        {
            EventBusManager.Instance.Raise(EventNameEnum.LevelChanged, GetXPProgressNormalized());
        }
        EventBusManager.Instance.Raise(EventNameEnum.XPChanged, GetXPProgressNormalized());
    }

    private float GetXPProgressNormalized()
    {
        if (Level >= MaxLevel)
            return 1f;

        return Mathf.Clamp01((float)CurrentXP / GetXPToNextLevel());
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

    private void OnBuyLevelXP_PlayerLevel(object[] parameters)
    {
        BuyXP(XPPerCoin);
    }
}