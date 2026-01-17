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

    public void DeployUnit(InventoryUnitCard card)
    {
        GameplayManager.Instance.InstantiateUnit(card.unitData, TeamEnum.Team1);
        _currentUnitsInInventory.Remove(card.unitData);
        UIManager.Instance.RemoveInventoryUnitCard(card);
    }
}
