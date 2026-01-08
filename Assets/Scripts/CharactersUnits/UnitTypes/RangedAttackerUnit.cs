using UnityEngine;

public class RangedAttackerUnit : BaseUnit
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
            }
        }
        else
        {
            GetInRange();
        }
    }
}
