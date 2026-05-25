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
    private CurrencyService _currencyServiceObj;
    private InventoryService _inventoryServiceObj;
    private TeamService _teamServiceObj;

    public ShopService(UnitScriptableObject unit_SO)
    {
        SubscribeToEvents();
        _allUnits = new List<UnitData>(unit_SO.unitDataList);
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
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
        UIManager.Instance.RemoveAllShopUnitCards();

        for (int i = 0; i < SHOP_SIZE; i++)
        {
            AddRandomUnitInShop();
        }
    }

    void AddRandomUnitInShop()
    {
        if (_currentUnitsInShop.Count >= SHOP_SIZE) return;

        UnitData randomUnit = _allUnits[Random.Range(0, _allUnits.Count)];
        _currentUnitsInShop.Add(randomUnit);

        UIManager.Instance.AddShopUnitCard(randomUnit);
    }

    public void BuyUnit(ShopUnitCard card)
    {    
        int cost = card.unitData.unitCost;

        if (_inventoryServiceObj.CurrentInventorySize >= _inventoryServiceObj.MaxInventorySize)
        {
            Debug.Log("Inventory Full!");
            return;
        }

        if (!_currencyServiceObj.SpendCurrency(cost))
        {
            Debug.Log("Not enough Currency!");
            return;
        }

        _teamServiceObj.AddUnitToTeam(card.unitData, TeamEnum.Team1);
        _inventoryServiceObj.AddUnit(card.unitData);
        _currentUnitsInShop.Remove(card.unitData);
        UIManager.Instance.RemoveShopUnitCard(card);
        AddRandomUnitInShop();
    }

    public void RefreshShop()
    {
        if (!_currencyServiceObj.SpendCurrency(_shopRefreshCost))
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
        _shopRefreshCost = (int)parameters[8];
        UIManager.Instance.UpdateRefreshCostUI(_shopRefreshCost);
    }

    public void Reset()
    {
        _currentUnitsInShop.Clear();
        _shopRefreshCost = 0;
    }

    public void Dispose()
    {
        UnsubscribeToEvents();

        Reset();

        if (_allUnits != null)
        {
            _allUnits.Clear();
            _allUnits = null;
        }
        _currencyServiceObj = null;
        _inventoryServiceObj = null;
        _teamServiceObj = null;
    }
}
