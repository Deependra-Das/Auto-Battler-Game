using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Spartan_RangedSupportUnit : BaseUnit
{
    private RangedAbilityPoolService _rangedAbilityPoolService;

    public override void Initialize(UnitData unitData, TeamEnum team, Node spawnNode)
    {
        base.Initialize(unitData, team, spawnNode);

        _rangedAbilityPoolService = GameManager.Instance.Get<RangedAbilityPoolService>();
    }

    protected override void Attack()
    {
        if (!canAttack || isAttacking || currentTarget == null) return;

        Vector3 direction = (currentTarget.CurrentNode.worldPosition - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        SetDirectionFacing(dirNormalized);
        isAttacking = true;
        attackRoutine = StartCoroutine(PerformSpartanElementalBurst());
        attackRoutine = null;
    }

    private IEnumerator PerformSpartanElementalBurst()
    {
        yield return null;
        animator.SetTrigger("Attack");
        _rangedAbilityPoolService.SpawnElementalBurst(this, currentTarget, totalDamage, unitData.unitElement, UnitData.attackAnimationDelay);
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
