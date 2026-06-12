using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Viking_RangedAttackerUnit : BaseUnit
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
        attackRoutine = StartCoroutine(ShootVikingArrowCoroutine());
        attackRoutine = null;
    }

    private IEnumerator ShootVikingArrowCoroutine()
    {
        yield return null;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(UnitData.attackAnimationDelay);
        _rangedAbilityPoolService.SpawnElementalArrow(this, currentTarget, totalDamage, unitData.unitElement);
        isAttacking = false;
        cooldownRoutine = StartCoroutine(AttackCoolDownWaitCoroutine());
        cooldownRoutine = null;
    }
}