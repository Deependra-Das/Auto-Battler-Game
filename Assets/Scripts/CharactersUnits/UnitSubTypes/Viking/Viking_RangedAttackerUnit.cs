using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Viking_RangedAttackerUnit : RangedAttackerUnit
{
    protected override void Attack()
    {
        if (!canAttack) return;

        PerformArrowAttack();
    }

    private void PerformArrowAttack()
    {
        Vector3 direction = (currentTarget.CurrentNode.position - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);

        SetDirectionFacing(dirNormalized);

        animator.SetTrigger("Attack");
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }

    private void ShootArrow()
    {
        GameManager.Instance.Get<RangedAbilityService>().SpawnArrow(this, currentTarget, baseDamage);
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
