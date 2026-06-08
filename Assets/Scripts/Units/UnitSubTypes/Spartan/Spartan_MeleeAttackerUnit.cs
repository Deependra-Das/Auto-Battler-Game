using System.Collections;
using UnityEngine;

public class Spartan_MeleeAttackerUnit : BaseUnit
{
    protected override void Attack()
    {
        if (!canAttack || isAttacking || currentTarget == null) return;

        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        isAttacking = true;
        StartCoroutine(PerformSpartanSwordSlashCoroutine());
    }

    private IEnumerator PerformSpartanSwordSlashCoroutine()
    {
        yield return null;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(unitData.attackAnimationDelay);
        DealDamage();
        isAttacking = false;
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }
}
