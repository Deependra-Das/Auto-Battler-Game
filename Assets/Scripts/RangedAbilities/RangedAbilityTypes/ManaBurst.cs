using System.Collections;
using UnityEngine;

public class ManaBurst : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private float _damageDelay;
    private UnitElementEnum _element;

    public void Initialize(
        BaseUnit targetUnit,
        int damage,
        UnitElementEnum attackElement,
        float lifetime,
        float damageDelay)
    {
        _targetUnit = targetUnit;
        _damage = damage;
        _element = attackElement;
        _lifetime = lifetime;
        _damageDelay = damageDelay;

        StartCoroutine(Execute());
        Destroy(gameObject, _lifetime);
    }

    private IEnumerator Execute()
    {
        yield return new WaitForSeconds(_damageDelay);
        ApplyDamage();
    }

    private void ApplyDamage()
    {
        if (_targetUnit == null || _targetUnit.IsDead)
            return;

        _targetUnit.TakeDamage(_damage, _element);
    }
}