using System.Collections;
using UnityEngine;

public class ElementalBurst : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private UnitElementEnum _element;

    public void Initialize(BaseUnit targetUnit, int damage, UnitElementEnum attackElement, float lifetime)
    {
        _targetUnit = targetUnit;
        _damage = damage;
        _element = attackElement;
        _lifetime = lifetime;

        StartCoroutine(Execute());
        Destroy(gameObject, _lifetime);
    }

    private IEnumerator Execute()
    {
        yield return null;
        ApplyDamage();
    }

    private void ApplyDamage()
    {
        if (_targetUnit == null || _targetUnit.IsDead)
            return;

        _targetUnit.TakeDamage(_damage, _element);
    }
}