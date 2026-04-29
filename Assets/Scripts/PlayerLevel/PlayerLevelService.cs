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

    private int _xpExchangeCost = 0;
    private int _xpExchangeValue = 0;

    public PlayerLevelService(PlayerLevelConfigScriptableObjectScript config)
    {
        SubscribeToEvents();
        _currencyService = GameManager.Instance.Get<CurrencyService>();
        _config = config;
    }

    ~PlayerLevelService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStarted_PlayerLevel);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStarted_PlayerLevel);
    }

    public int MaxLevel => _config.playerProgressionDataList.Count;

    public int MaxUnitsAllowedOnField => GetCurrentPlayerLevelData().maxUnitsAllowed;

    public PlayerLevelData GetCurrentPlayerLevelData()
    {
        int index = Level - 1;
        return _config.playerProgressionDataList[index];
    }

    private int GetXPToNextLevel()
    {
        if (Level >= MaxLevel)
            return 0;

        int index = Level - 1;
        return _config.playerProgressionDataList[index].xpRequiredToNextLevel;
    }

    private bool BuyXP()
    {
        if (Level >= MaxLevel)
            return false;

        if (!_currencyService.SpendCurrency(_xpExchangeCost))
            return false;

        int xpGained = _xpExchangeCost * _xpExchangeValue;
        CurrentXP += xpGained;

        HandleLevelUp();

        if (CurrentXP > 0)
        {
            EventBusManager.Instance.Raise(EventNameEnum.XPChanged, GetXPProgressNormalized(), CurrentXP, GetXPToNextLevel());
        }

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

            Level++;
            CurrentXP = 0;
            leveledUp = true;
        }

        if (Level >= MaxLevel)
        {
            Level = MaxLevel;
            CurrentXP = 0;
        }
        if (leveledUp)
        {
            int index = Level - 1;
            EventBusManager.Instance.Raise(EventNameEnum.LevelChanged, Level, MaxUnitsAllowedOnField, CurrentXP, GetXPToNextLevel());
        }
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

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetXP(int xp)
    {
        CurrentXP = xp;
    }

    public void Reset()
    {
        Level = 1;
        Lives = 3;
        CurrentXP = 0;
    }

    private void OnBuyLevelXP_PlayerLevel(object[] parameters)
    {
        BuyXP();
    }

    private void OnStageStarted_PlayerLevel(object[] parameters)
    {
        _xpExchangeCost = (int)parameters[5];
        _xpExchangeValue = (int)parameters[6];
    }
}