using System.Collections;
using UnityEngine;

public class Viking_MeleeTankUnit : BaseUnit
{
    protected override void Attack()
    {
        if (!canAttack) return;

        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);

        PerformWarAxeAttack();
    }

    private void PerformWarAxeAttack()
    {
        animator.SetTrigger("Attack");
        currentTarget.TakeDamage(unitData.baseDamage, unitData.unitElement);
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }
}
