using AutoBattler.Event;
using AutoBattler.Main;
using System.Collections.Generic;
using UnityEngine;

public class ShopService
{
    private List<UnitData> _allUnits;
    private List<UnitData> _currentUnitsInShop = new List<UnitData>();
    private const int SHOP_SIZE = 4;
    private int _shopRefreshCost = 0;
    CurrencyService currencyServiceObj;
    InventoryService inventoryServiceObj;

    public ShopService(UnitScriptableObject unit_SO)
    {
        SubscribeToEvents();
        _allUnits = new List<UnitData>(unit_SO.unitDataList);
        currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
    }

    ~ShopService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStarted_Shop);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStarted_Shop);
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
        int cost = card.unitData.unitCost;

        if (inventoryServiceObj.CurrentInventorySize >= inventoryServiceObj.MaxInventorySize)
        {
            Debug.Log("Inventory Full!");
            return;
        }

        if (!currencyServiceObj.SpendCurrency(cost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        inventoryServiceObj.AddUnit(card.unitData);
        _currentUnitsInShop.Remove(card.unitData);
        UIManager.Instance.RemoveShopUnitCard(card);
        AddRandomUnitInShop();
    }

    public void RefreshShop()
    {
        if (!currencyServiceObj.SpendCurrency(_shopRefreshCost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        _currentUnitsInShop.Clear();
        UIManager.Instance.RemoveAllShopUnitCards();

        GenerateShopUnits();
    }

    private void OnStageStarted_Shop(object[] parameters)
    {
        _shopRefreshCost = (int)parameters[7];
        UIManager.Instance.UpdateRefreshCostUI(_shopRefreshCost);
    }
}
