using System.Collections;
using UnityEngine;

public class Spartan_MeleeTankUnit : BaseUnit
{
    protected override void Attack()
    {
        if (!canAttack || isAttacking || currentTarget == null) return;

        Vector3 direction = (currentTarget.CurrentNode.worldPosition - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        isAttacking = true;
        attackRoutine = StartCoroutine(PerformSpartanPolearmThrustCoroutine());
        attackRoutine = null;
    }

    private IEnumerator PerformSpartanPolearmThrustCoroutine()
    {
        yield return null;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(unitData.attackAnimationDelay);
        DealDamage();
        isAttacking = false;
        cooldownRoutine = StartCoroutine(AttackCoolDownWaitCoroutine());
        cooldownRoutine = null;
    }
}
