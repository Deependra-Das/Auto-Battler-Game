using UnityEngine;

public class RangedAbilityService
{
    private Arrow _arrowPrefab;
    private ManaBurst _magicBurstPrefab;

    private float arrowOffset = 0.35f;

    public RangedAbilityService(RangedAbilitiesScriptableObjectScript rangedAbilities_SO)
    {
        _arrowPrefab = rangedAbilities_SO.arrowPrefab;
        _magicBurstPrefab = rangedAbilities_SO.manaBurstPrefab;
    }

    public void SpawnArrow(BaseUnit owner, BaseUnit target, int damage)
    {
        if (target == null) return;

        Vector3 spawnPosition = owner.transform.position;
        Vector3 adjustedTargetPosition = target.transform.position;

        if (owner.DirectionFacing == UnitFacingDirectionEnum.Left || owner.DirectionFacing == UnitFacingDirectionEnum.Right)
        {
            spawnPosition += new Vector3(0, arrowOffset, 0);
            adjustedTargetPosition += new Vector3(0, arrowOffset, 0);
        }

        Arrow arrow = GameObject.Instantiate(_arrowPrefab, spawnPosition, Quaternion.identity);

        Vector3 adjustedDirection = (adjustedTargetPosition - spawnPosition);
        arrow.Initialize(owner, target, damage, adjustedDirection.normalized, adjustedTargetPosition);
    }

    public void SpawnManaBurst(BaseUnit owner, BaseUnit target, int damage, float lifetime, float damageDelay)
    {
        if (target == null) return;
        ManaBurst circle = GameObject.Instantiate(_magicBurstPrefab, target.transform.position, Quaternion.identity);
        circle.Initialize(target, damage, lifetime, damageDelay);
    }
}