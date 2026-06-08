using System;
using UnityEngine;

[Serializable]
public struct UnitData
{
    public int unitID;
    public Sprite unitIcon;
    public string unitName;
    public UnitFactionEnum unitFaction;
    public UnitTypeEnum unitType;
    public UnitElementEnum unitElement;
    [Range(1,3)] public int unitLevel;
    [Range(1, 5)] public int baseRange;

    public int baseUnitCost;
    public int baseDamage;
    public int baseHealth;
    public int baseShield;
    public int baseHealing;
    public float baseAttackSpeed;
    public float baseMovementSpeed;
    public float attackAnimationDelay;
    public float baseElementalDamageScalingFactor;
    public float teamShieldScalingFactor;
}
