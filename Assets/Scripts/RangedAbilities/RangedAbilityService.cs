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

    public void SpawnRangedAbility(RangedAbilityTypeEnum type, BaseUnit owner, BaseUnit target, int damage)
    {
        if (target == null) return;

        switch (type)
        {
            case RangedAbilityTypeEnum.Arrow:
                SpawnArrow(owner, target, damage);
                break;

            case RangedAbilityTypeEnum.ManaBurst:
                SpawnMagicCircle(target, damage);
                break;
        }
    }

    private void SpawnArrow(BaseUnit owner, BaseUnit target, int damage)
    {
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

    private void SpawnMagicCircle(BaseUnit target, int damage)
    {
        ManaBurst circle = GameObject.Instantiate(_magicBurstPrefab, target.transform.position, Quaternion.identity);
        circle.Initialize(target, damage);
    }
}