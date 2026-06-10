using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Crusader_RangedSupportUnit : BaseUnit
{
    protected override void Attack()
    {
        if (!canAttack || isAttacking || currentTarget == null) return;

        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        SetDirectionFacing(dirNormalized);
        isAttacking = true;
        attackRoutine = StartCoroutine(PerformCrusaderElementalBurst());
        attackRoutine = null;
    }

    private IEnumerator PerformCrusaderElementalBurst()
    {
        yield return null;
        animator.SetTrigger("Attack");
        GameManager.Instance.Get<RangedAbilityService>().SpawnElementalBurst(this, currentTarget, totalDamage, unitData.unitElement, UnitData.attackAnimationDelay);
        yield return new WaitForSeconds(UnitData.attackAnimationDelay);
        HealAllTeammates();
        isAttacking = false;
        cooldownRoutine = StartCoroutine(AttackCoolDownWaitCoroutine());
        cooldownRoutine = null;
    }

    protected void HealAllTeammates()
    {
        var teammates = GameManager.Instance.Get<TeamService>().GetFieldUnits(Team);

        foreach (BaseUnit unit in teammates)
        {
            if (unit == null || unit.IsDead) continue;
            unit.Heal(unitData.baseHealing);
        }
    }
}
