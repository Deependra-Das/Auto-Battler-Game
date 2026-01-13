using System.Collections.Generic;
using UnityEngine;

public class InventoryService
{
    private List<UnitData> _currentUnitsInInventory = new List<UnitData>();

    public void AddUnit(UnitData unitData)
    {
        _currentUnitsInInventory.Add(unitData);
        UIManager.Instance.AddInventoryUnitCard(unitData);
    }

    public void RemoveUnit(UnitData unitData)
    {
        _currentUnitsInInventory.Remove(unitData);
    }
}
