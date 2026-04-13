using UnityEngine;

public class ManaBurst : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private float _damageDelay;
    private UnitElementEnum _element;

    public void Initialize(BaseUnit targetUnit, int damage, UnitElementEnum attackElement, float lifetime, float damageDelay)
    {
        _targetUnit = targetUnit;
        _damage = damage;
        _element = attackElement;
        _lifetime = lifetime;
        _damageDelay = damageDelay;
        Invoke(nameof(ApplyDamage), _damageDelay);
        Destroy(gameObject, _lifetime);
    }

    private void ApplyDamage()
    {
        if (_targetUnit == null)
            return;

        _targetUnit.TakeDamage(_damage,_element);
    }
}
