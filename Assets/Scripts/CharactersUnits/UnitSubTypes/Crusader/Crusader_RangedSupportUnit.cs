using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Crusader_RangedSupportUnit : RangedSupportUnit
{
    protected override void Attack()
    {
        if (!canAttack) return;

        PerformManaAttack();
    }

    private void PerformManaAttack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCoolDownWaitCoroutine());
    }

    private void ManaBurst()
    {
        GameManager.Instance.Get<RangedAbilityService>().SpawnManaBurst(this, currentTarget, baseDamage, lifetime, damageDelay);
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
