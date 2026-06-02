using System;
using UnityEngine;

[Serializable]
public struct UnitData
{
    public int unitID;
    public BaseUnit unitPrefab;
    public Sprite unitIcon;
    public string unitName;
    public UnitFactionEnum unitFaction;
    public UnitTypeEnum unitType;
    [Range(1,3)] public int unitLevel;
    public UnitElementEnum unitElement;
    public int unitCost;

    public int baseDamage;
    public int baseHealth;
    public int baseShield;
    public int baseHealing;
    public float baseAttackSpeed;
    public float baseMovementSpeed;
    [Range(1, 5)] public int baseRange;
    public float delayBeforeRangedAttack;
    public float baseElementalDamageScalingFactor;
    public float teamShieldScalingFactor;
}
