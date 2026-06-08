using UnityEngine;

public class RangedAbilityService
{
    private ElementalArrow _arrowPrefab;
    private ElementalBurst _elementalBurstPrefab;
    private float _elementalBurstLifetime;
    private float _arrowLifetime;
    private float _arrowOffset;

    public RangedAbilityService(RangedAbilitiesScriptableObjectScript rangedAbilities_SO)
    {
        _arrowPrefab = rangedAbilities_SO.arrowPrefab;
        _elementalBurstPrefab = rangedAbilities_SO.elementalBurstPrefab;
        _arrowLifetime = rangedAbilities_SO.arrowLifetime;
        _elementalBurstLifetime = rangedAbilities_SO.elementalBurstLifetime;
        _arrowOffset = rangedAbilities_SO.arrowOffset;
    }

    public void SpawnElementalArrow(BaseUnit owner, BaseUnit target, int damage, UnitElementEnum attackElement)
    {
        if (target == null) return;

        Vector3 spawnPosition = owner.transform.position;
        Vector3 adjustedTargetPosition = target.transform.position;

        if (owner.DirectionFacing == UnitFacingDirectionEnum.Left || owner.DirectionFacing == UnitFacingDirectionEnum.Right)
        {
            spawnPosition += new Vector3(0, _arrowOffset, 0);
            adjustedTargetPosition += new Vector3(0, _arrowOffset, 0);
        }

        ElementalArrow arrow = GameObject.Instantiate(_arrowPrefab, spawnPosition, Quaternion.identity);

        Vector3 adjustedDirection = (adjustedTargetPosition - spawnPosition);
        arrow.Initialize(owner, target, damage, attackElement, adjustedDirection.normalized, adjustedTargetPosition, _arrowLifetime);
    }

    public void SpawnElementalBurst(BaseUnit owner, BaseUnit target, int damage, UnitElementEnum attackElement, float damageDelay)
    {
        if (target == null) return;
        ElementalBurst circle = GameObject.Instantiate(_elementalBurstPrefab, target.CurrentNode.position, Quaternion.identity);
        circle.Initialize(target, damage, attackElement, _elementalBurstLifetime);
    }
}