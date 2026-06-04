using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPoolService
{
    private Transform _pooledUnitContainerTransform;
    private readonly Dictionary<int, BaseUnit> _prefabLookup;
    private readonly Dictionary<int, Queue<BaseUnit>> _pooledUnitDictionary = new();

    public UnitPoolService(UnitPrefabScriptableObjectScript unitPrefab_SO)
    {
        _prefabLookup = new Dictionary<int, BaseUnit>();
        BuildLookup(unitPrefab_SO);
    }

    public void BuildLookup(UnitPrefabScriptableObjectScript unitPrefab_SO)
    {
        foreach (var entry in unitPrefab_SO.unitPrefabList)
        {
            if (entry == null || entry.unitPrefab == null)
                continue;

            _prefabLookup[entry.unitID] = entry.unitPrefab;
        }
    }

    public BaseUnit Get(int unitID)
    {
        if (!_pooledUnitDictionary.TryGetValue(unitID, out Queue<BaseUnit> pooledUnitQueue))
        {
            pooledUnitQueue = new Queue<BaseUnit>();
            _pooledUnitDictionary[unitID] = pooledUnitQueue;
        }

        if (pooledUnitQueue.Count > 0)
        {
            return pooledUnitQueue.Dequeue();
        }

        return CreateNew(unitID);
    }

    public BaseUnit CreateNew(int unitID)
    {

        if (!_prefabLookup.TryGetValue(unitID, out BaseUnit prefab))
        {
            Debug.LogError($"Missing prefab for {unitID}");
            return null;         
        }

        BaseUnit newUnit = GameObject.Instantiate(prefab, _pooledUnitContainerTransform);
        newUnit.gameObject.SetActive(false);
        return newUnit;
    }

    public void Release(int unitID, BaseUnit unit)
    {
        if (unit == null) return;

        //unit.ResetUnit();
        unit.gameObject.SetActive(false);

        if (!_pooledUnitDictionary.TryGetValue(unitID, out Queue<BaseUnit> pooledUnitQueue))
        {
            pooledUnitQueue = new Queue<BaseUnit>();
            _pooledUnitDictionary[unitID] = pooledUnitQueue;
        }

        pooledUnitQueue.Enqueue(unit);
    }
}
