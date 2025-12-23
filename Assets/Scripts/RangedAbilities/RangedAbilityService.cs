using UnityEngine;

public class RangedAbilityService
{
    private Arrow _arrowPrefab;
    private ManaBurst _magicBurstPrefab;

    public RangedAbilityService(RangedAbilitiesScriptableObjectScript rangedAbilities_SO)
    {
        _arrowPrefab = rangedAbilities_SO.arrowPrefab;
        _magicBurstPrefab = rangedAbilities_SO.manaBurstPrefab;
    }

    public void SpawnProjectile(RangedAbilityTypeEnum type, BaseUnit owner, BaseUnit target, int damage)
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
        Arrow arrow = GameObject.Instantiate(_arrowPrefab, owner.transform.position, Quaternion.identity);
        arrow.Initialize(target, damage);
    }

    private void SpawnMagicCircle(BaseUnit target, int damage)
    {
        ManaBurst circle = GameObject.Instantiate(_magicBurstPrefab, target.transform.position, Quaternion.identity);
        circle.Initialize(target, damage);
    }
}