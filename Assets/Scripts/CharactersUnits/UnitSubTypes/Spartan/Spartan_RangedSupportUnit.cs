using AutoBattler.Main;
using System.Collections;
using UnityEngine;

public class Spartan_RangedSupportUnit : RangedSupportUnit
{
    [SerializeField] protected float lifetime = 1f;
    [SerializeField] protected float damageDelay = 0.2f;

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
        HealAllTeammates();
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
