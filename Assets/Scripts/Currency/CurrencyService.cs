using AutoBattler.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrencyService
{
    public int Balance { get; private set; }

    public CurrencyService()
    {
        SubscribeToEvents();
        Balance = 0;
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStartedSetInitialCurrency);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOver, OnRoundOverAddCurrency);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStartedSetInitialCurrency);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOver, OnRoundOverAddCurrency);
    }

    private void OnStageStartedSetInitialCurrency(object[] parameters)
    {
        SetCurrency((int)parameters[9]);
    }

    private void OnRoundOverAddCurrency(object[] parameters)
    {
        AddCurrency((int)parameters[2]);
    }

    public bool CanAfford(int amount)
    {
        return Balance >= amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= 0 || !CanAfford(amount))
        {
            return false;
        }

        Balance -= amount;
        NotifyBalanceChanged();
        return true;
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;
        NotifyBalanceChanged();
    }

    private void SetCurrency(int amount)
    {
        Balance = amount;
        NotifyBalanceChanged();
    }

    private void NotifyBalanceChanged()
    {
        UIManager.Instance.UpdateCurrenyUI(Balance);
    }

    public void Reset()
    {
        Balance = 0;
        NotifyBalanceChanged();
    }

    public void Dispose()
    {
        UnsubscribeToEvents();
        Reset();
    }
}
