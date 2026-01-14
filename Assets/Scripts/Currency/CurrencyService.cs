using UnityEngine;

public class CurrencyService
{
    public int Balance { get; private set; }

    public CurrencyService(int startingCurrencyAmount)
    {
        Balance = startingCurrencyAmount;
    }

    public bool CanAfford(int amount)
    {
        return Balance >= amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Balance -= amount;
        UIManager.Instance.UpdateCurrenyUI(Balance);
        return true;
    }

    public void AddCurrency(int amount)
    {
        Balance += amount;
        UIManager.Instance.UpdateCurrenyUI(Balance);
    }
}
