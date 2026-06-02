using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitDataService
{
    private Dictionary<int, UnitData> _unitDatabase = new Dictionary<int, UnitData>();

    public UnitDataService(UnitScriptableObject unit_SO)
    {
        BuildUnitDatabase(unit_SO);
    }

    private void BuildUnitDatabase(UnitScriptableObject unit_SO)
    {
        _unitDatabase = unit_SO.unitDataList.ToDictionary(unit => unit.unitID, unit => unit);
    }

    public bool TryGetUnitDataById(int unitId, out UnitData unitData)
    {
        if (_unitDatabase.TryGetValue(unitId, out unitData))
        {
            return true;
        }

        Debug.LogWarning($"Unit ID {unitId} not found in database.");
        return false;
    }

    public bool TryGetUnitDataByIndex(int index, out UnitData unitData)
    {
        unitData = default;

        if (_unitDatabase == null)
        {
            Debug.LogWarning("Unit database is null.");
            return false;
        }

        if (index < 0 || index >= _unitDatabase.Count)
        {
            Debug.LogWarning($"Index {index} is out of range.");
            return false;
        }

        unitData = _unitDatabase.Values.ElementAt(index);
        return true;
    }

    public void Dispose()
    {
        _unitDatabase.Clear();
        _unitDatabase = null;
    }

    public int GetUnitDatabaseSize()
    {
        return _unitDatabase.Count;
    }
}

