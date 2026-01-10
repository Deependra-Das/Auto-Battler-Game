using System.Collections;
using UnityEngine;

public class Crusader_MeleeAttackerUnit : MeleeAttackerUnit
{
    protected override void Attack()
    {
        if (!canAttack) return;

        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);

        PerformSwordAttack();
    }

    private void PerformSwordAttack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }

    IEnumerator AttackCoolDownWaitCoroutine()
    {
        canAttack = false;
        yield return null;
        animator.ResetTrigger("Attack");
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }
}
