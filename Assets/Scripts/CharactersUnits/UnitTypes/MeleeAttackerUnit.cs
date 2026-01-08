using UnityEngine;

public class MeleeAttackerUnit : BaseUnit
{
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
                currentTarget.TakeDamage(baseDamage);
            }
        }
        else
        {
            GetInRange();
        }
    }
}
