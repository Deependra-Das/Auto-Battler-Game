using UnityEngine;

public class CurrencyService
{
    public int Balance { get; private set; }

    public CurrencyService(int startingCurrencyAmount)
    {
        Balance =startingCurrencyAmount;
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
