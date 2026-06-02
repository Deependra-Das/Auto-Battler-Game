using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Crusader_RangedSupportUnit : BaseUnit
{
    [SerializeField] protected float lifetime = 1f;
    [SerializeField] protected float damageDelay = 0.2f;

    protected override void Attack()
    {
        if (!canAttack) return;

        PerformElementalBurstAttack();
    }

    private void PerformElementalBurstAttack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }

    private void CrusaderElementalBurst()
    {
        GameManager.Instance.Get<RangedAbilityService>().SpawnManaBurst(this, currentTarget, totalDamage, unitData.unitElement, lifetime, damageDelay);
        HealAllTeammates();
    }

    protected void HealAllTeammates()
    {
        var teammates = GameManager.Instance.Get<TeamService>().GetFieldUnits(Team);

        foreach (BaseUnit unit in teammates)
        {
            unit.Heal(unitData.baseHealing);
        }
    }
}
