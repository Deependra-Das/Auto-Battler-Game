using System;
using UnityEngine;

[Serializable]
public struct UnitData
{
    public BaseUnit unitPrefab;
    public Sprite unitIcon;
    public string unitName;
    public UnitTypeEnum unitType;
    [Range(1,3)] public int unitLevel;
    public int unitCost;
}
