using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;

public class PlayerLevelService
{
    private CurrencyService _currencyService;
    private PlayerLevelConfigScriptableObjectScript _config;
    private int _defaultLevel = 1;
    private int _defaultLives = 3;

    public int Level { get; private set; }
    public int Lives { get; private set; }
    public int CurrentXP { get; private set; }

    private int _xpExchangeCost = 0;
    private int _xpExchangeValue = 0;

    public PlayerLevelService(PlayerLevelConfigScriptableObjectScript config)
    {
        SubscribeToEvents();
        _currencyService = GameManager.Instance.Get<CurrencyService>();
        _config = config;
        Reset();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStarted_PlayerLevel);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOver, OnRoundOver_PlayerLevel);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.BuyLevelXP, OnBuyLevelXP_PlayerLevel);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStarted_PlayerLevel);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOver, OnRoundOver_PlayerLevel);
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
            RaiseXPChangedEvent();
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
            RaiseLevelChangedEvent();
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

    private void SetLevel(int level)
    {
        Level = level;
    }

    private void SetCurrentXP(int xp)
    {
        CurrentXP = xp;
    }

    private void SetLives(int lives)
    {
        Lives = lives;
    }

    private void OnBuyLevelXP_PlayerLevel(object[] parameters)
    {
        BuyXP();
    }

    private void OnStageStarted_PlayerLevel(object[] parameters)
    {
        SetLevel((int)parameters[3]);
        SetCurrentXP((int)parameters[4]);
        SetLives((int)parameters[5]);
        _xpExchangeCost = (int)parameters[6];
        _xpExchangeValue = (int)parameters[7];

        if (CurrentXP > 0)
        {
            RaiseXPChangedEvent();
        }

        RaiseLevelChangedEvent();
    }

    private void OnRoundOver_PlayerLevel(object[] parameters)
    {
        RoundResultEnum result = (RoundResultEnum)parameters[2];

        if(result == RoundResultEnum.Lose)
        {
            LoseLife();
        }
    }

    public void Reset()
    {
        Level = _defaultLevel;
        Lives = _defaultLives;
        CurrentXP = 0;

        _xpExchangeCost = 0;
        _xpExchangeValue = 0;

        RaiseLevelChangedEvent();
    }

    private void RaiseXPChangedEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.XPChanged, GetXPProgressNormalized(), CurrentXP, GetXPToNextLevel());
    }

    private void RaiseLevelChangedEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.LevelChanged, Level, MaxUnitsAllowedOnField, CurrentXP, GetXPToNextLevel());
    }

    public void Dispose()
    {
        UnsubscribeToEvents();

        Reset();

        _currencyService = null;
        _config = null;
    }
}