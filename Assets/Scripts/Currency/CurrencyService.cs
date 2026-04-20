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

    ~CurrencyService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStartedSetInitialiCurrency);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStartedSetInitialiCurrency);
    }

    private void OnStageStartedSetInitialiCurrency(object[] parameters)
    {
        Balance = (int)parameters[2];
        NotifyBalanceChanged();
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

    private void NotifyBalanceChanged()
    {
        UIManager.Instance.UpdateCurrenyUI(Balance);
    }
}
