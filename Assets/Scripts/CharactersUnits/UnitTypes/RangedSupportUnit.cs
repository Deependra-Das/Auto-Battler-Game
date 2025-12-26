using UnityEngine;

public class RangedSupportUnit : BaseUnit
{
    [SerializeField] protected float lifetime = 1f;
    [SerializeField] protected float damageDelay = 0.2f;

    protected void Update()
    {
        if (!HasEnemy)
        {
            FindTarget();
        }

        if (isTargetInRange && !isMoving)
        {
            if (canAttack)
            {
                Attack();
            }
        }
        else
        {
            GetInRange();
        }
    }
}
