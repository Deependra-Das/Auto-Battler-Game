using System.Collections;
using UnityEngine;

public class Spartan_MeleeAttackerUnit : BaseUnit
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

    private void SpartanSwordSlash()
    {
        currentTarget.TakeDamage(unitData.baseDamage, unitData.unitElement);
    }
}
