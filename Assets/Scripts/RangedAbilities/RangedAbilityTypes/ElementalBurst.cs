using System.Collections;
using UnityEngine;

public class ElementalBurst : MonoBehaviour
{
    private BaseUnit _ownerUnit;
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private UnitElementEnum _element;
    private RangedAbilityPoolService _rangedAbilityPoolServiceObj;
    private Coroutine _executeCoroutine;

    public void Initialize(BaseUnit ownerUnit, BaseUnit targetUnit, int damage, UnitElementEnum attackElement, float lifetime, RangedAbilityPoolService rangedAbilityPoolServiceObj)
    {
        _ownerUnit = ownerUnit;
        _targetUnit = targetUnit;
        _damage = damage;
        _element = attackElement;
        _lifetime = lifetime;
        _rangedAbilityPoolServiceObj = rangedAbilityPoolServiceObj;

        if (_executeCoroutine != null)
            StopCoroutine(_executeCoroutine);

        _executeCoroutine = StartCoroutine(Execute());
    }

    private IEnumerator Execute()
    {
        yield return new WaitForSeconds(_lifetime);
        ApplyDamage();
        _rangedAbilityPoolServiceObj.DespawnElementalBurst(this);
    }

    private void ApplyDamage()
    {
        if (_targetUnit == null || _targetUnit.IsDead)
            return;

        _targetUnit.TakeDamage(_damage, _element);
    }

    public void Reset()
    {
        if (_executeCoroutine != null)
            StopCoroutine(_executeCoroutine);

        _executeCoroutine = null;
        _ownerUnit = null;
        _targetUnit = null;
        _damage = 0;
        _element = default;
        _lifetime = 0;
        _rangedAbilityPoolServiceObj = null;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}