using AutoBattler.Main;
using UnityEngine;

public class RangedSupportUnit : BaseUnit
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

    protected void HealAllTeammates()
    {
        var teammates = GameManager.Instance.Get<TeamService>().GetFieldUnits(Team);

        foreach (BaseUnit unit in teammates)
        {
            unit.Heal(baseHealing);
        }
    }
}
