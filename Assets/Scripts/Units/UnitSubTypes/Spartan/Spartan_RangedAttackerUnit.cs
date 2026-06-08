using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Spartan_RangedAttackerUnit : BaseUnit
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
        StartCoroutine(ShootSpartanArrowCoroutine());
    }

    private IEnumerator ShootSpartanArrowCoroutine()
    {
        yield return null;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(UnitData.attackAnimationDelay);
        GameManager.Instance.Get<RangedAbilityService>().SpawnArrow(this, currentTarget, totalDamage, unitData.unitElement);
        StartCoroutine(AttackCoolDownWaitCoroutine());
        isAttacking = false;
    }
}
