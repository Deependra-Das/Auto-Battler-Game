using UnityEngine;

public class RangedAbilityService
{
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private ManaBurst _magicCirclePrefab;

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
        ManaBurst circle = GameObject.Instantiate(_magicCirclePrefab, target.transform.position, Quaternion.identity);
        circle.Initialize(target, damage);
    }
}