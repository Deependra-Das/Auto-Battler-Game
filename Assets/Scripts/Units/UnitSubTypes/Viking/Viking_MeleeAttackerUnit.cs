using System.Collections;
using UnityEngine;

public class Viking_MeleeAttackerUnit : BaseUnit
{
    protected override void Attack()
    {
        if (!canAttack || isAttacking || currentTarget == null) return;

        Vector3 direction = (currentTarget.CurrentNode.worldPosition - this.transform.position);
        Vector3 dirNormalized = direction.normalized;
        animator.SetFloat("MoveX", dirNormalized.x);
        animator.SetFloat("MoveY", dirNormalized.y);
        isAttacking = true;
        attackRoutine = StartCoroutine(PerformVikingSpearAxeSlashCoroutine());
        attackRoutine = null;
    }

    private IEnumerator PerformVikingSpearAxeSlashCoroutine()
    {
        yield return null;
        animator.SetTrigger("Attack");
        AudioManager.Instance.PlayMeleeAttackAudio();
        yield return new WaitForSeconds(unitData.attackAnimationDelay);
        AudioManager.Instance.PlayThunderAttackAudio();
        SpawnElementalVfx();
        DealDamage();
        isAttacking = false;
        cooldownRoutine = StartCoroutine(AttackCoolDownWaitCoroutine());
        cooldownRoutine = null;
    }

    public override void PlayFootstep()
    {
        AudioManager.Instance.PlayFootstepAudio();
    }
}
