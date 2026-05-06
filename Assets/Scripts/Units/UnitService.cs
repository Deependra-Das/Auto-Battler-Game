using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitService
{
    private Dictionary<int, UnitData> _unitDatabase = new Dictionary<int, UnitData>();

    public UnitService(UnitScriptableObject unit_SO)
    {
        BuildUnitDatabase(unit_SO);
    }

    private void BuildUnitDatabase(UnitScriptableObject unit_SO)
    {
        _unitDatabase = unit_SO.unitDataList.ToDictionary(unit => unit.unitID, unit => unit);
    }

    public bool TryGetUnitById(int unitId, out UnitData unitData)
    {
        if (_unitDatabase.TryGetValue(unitId, out unitData))
        {
            return true;
        }

        Debug.LogWarning($"Unit ID {unitId} not found in database.");
        return false;
    }
}

