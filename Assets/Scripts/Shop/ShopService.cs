using AutoBattler.Main;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopService
{
    private List<UnitData> _allUnits;
    private List<UnitData> _currentUnitsInShop = new List<UnitData>();
    private const int SHOP_SIZE = 4;

    public ShopService(UnitScriptableObject unit_SO)
    {
        _allUnits = new List<UnitData>(unit_SO.unitDataList);
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

    public void BuyUnit(ShopUnitCard card, UnitData shopUnitData)
    {
        CurrencyService currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        if (!currencyServiceObj.SpendCurrency(shopUnitData.unitCost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        GameManager.Instance.Get<InventoryService>().AddUnit(shopUnitData);
        Debug.Log($"Bought {shopUnitData.unitID}");

        _currentUnitsInShop.Remove(shopUnitData);
        UIManager.Instance.RemoveShopUnitCard(card);
        AddRandomUnitInShop();
    }
}
