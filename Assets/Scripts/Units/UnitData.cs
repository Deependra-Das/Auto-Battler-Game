using System;
using UnityEngine;

[Serializable]
public struct UnitData
{
    [HideInInspector] public int unitID;
    public BaseUnit unitPrefab;
    public Sprite unitIcon;
    public string unitName;
    public UnitFactionEnum unitFaction;
    public UnitTypeEnum unitType;
    [Range(1,3)] public int unitLevel;
    public int unitCost;
}
