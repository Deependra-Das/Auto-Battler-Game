using AutoBattler.Main;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopService
{
    private List<UnitData> _allUnits;
    private List<UnitData> _currentUnitsInShop = new List<UnitData>();
    private const int SHOP_SIZE = 4;
    private int _shopRefreshCost = 0;

    public ShopService(UnitScriptableObject unit_SO, int shopRefreshCost)
    {
        _allUnits = new List<UnitData>(unit_SO.unitDataList);
        _shopRefreshCost = shopRefreshCost;
        UIManager.Instance.UpdateRefreshCostUI(_shopRefreshCost);
    }

    public void GenerateShopUnits()
    {
        _currentUnitsInShop.Clear();

        for (int i = 0; i < SHOP_SIZE; i++)
        {
            AddRandomUnitInShop();
        }
    }

    void AddRandomUnitInShop()
    {
        UnitData randomUnit = _allUnits[Random.Range(0, _allUnits.Count)];
        _currentUnitsInShop.Add(randomUnit);

        UIManager.Instance.AddShopUnitCard(randomUnit);
    }

    public void BuyUnit(ShopUnitCard card)
    {
        CurrencyService currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        if (!currencyServiceObj.SpendCurrency(card.unitData.unitCost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        GameManager.Instance.Get<InventoryService>().AddUnit(card.unitData);
        Debug.Log($"Bought {card.unitData.unitID}");

        _currentUnitsInShop.Remove(card.unitData);
        UIManager.Instance.RemoveShopUnitCard(card);
        AddRandomUnitInShop();
    }

    public void RefreshShop()
    {
        CurrencyService currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        if (!currencyServiceObj.SpendCurrency(_shopRefreshCost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        _currentUnitsInShop.Clear();
        UIManager.Instance.RemoveAllShopUnitCards();
        GenerateShopUnits();
    }
}
