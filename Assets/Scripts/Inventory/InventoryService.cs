using System.Collections.Generic;
using UnityEngine;

public class InventoryService
{
    private List<UnitData> _currentUnitsInInventory = new List<UnitData>();

    public int MaxInventorySize { get; private set; }

    public int CurrentInventorySize => _currentUnitsInInventory.Count;

    public void AddUnit(UnitData unitData)
    {
        _currentUnitsInInventory.Add(unitData);
        UIManager.Instance.AddInventoryUnitCard(unitData);
    }

    public void RemoveUnit(UnitData unitData)
    {
        _currentUnitsInInventory.Remove(unitData);
    }

    public void SetMaxInventorySize(int newSize)
    {
        MaxInventorySize = newSize;
    }

    public void DeployUnit(InventoryUnitCard card, Node node)
    {
        GameplayManager.Instance.InstantiateUnit(card.UnitData, node, TeamEnum.Team1);
        _currentUnitsInInventory.Remove(card.UnitData);
        UIManager.Instance.RemoveInventoryUnitCard(card);
    }

    public void ReorderUnits(List<UnitData> newOrder)
    {
        _currentUnitsInInventory.Clear();
        _currentUnitsInInventory.AddRange(newOrder);
    }

    public bool CanAddUnit => _currentUnitsInInventory.Count < MaxInventorySize;
}
