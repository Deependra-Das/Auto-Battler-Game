using UnityEngine;

public class RangedSupportUnit : BaseUnit
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
