using UnityEngine;

public class RangedAttackerUnit : BaseUnit
{
    protected void Update()
    {
        if (!isActive) return;

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
