using UnityEngine;

public class CurrencyService
{
    public int Balance { get; private set; }

    public CurrencyService(int startingCurrencyAmount)
    {
        AddCurrency(startingCurrencyAmount);
    }

    public bool CanAfford(int amount)
    {
        return Balance >= amount;
    }

    public void SpendCurrency(int amount)
    {
        Balance -= amount;
        UIManager.Instance.UpdateCurrenyUI(Balance);
    }

    public void AddCurrency(int amount)
    {
        Balance += amount;
        UIManager.Instance.UpdateCurrenyUI(Balance);
    }
}
