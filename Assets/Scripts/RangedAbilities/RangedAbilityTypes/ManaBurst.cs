using UnityEngine;

public class ManaBurst : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private float _damageDelay;

    public void Initialize(BaseUnit targetUnit, int damage, float lifetime, float damageDelay)
    {
        _targetUnit = targetUnit;
        _damage = damage;
        _lifetime = lifetime;
        _damageDelay = damageDelay;
        Invoke(nameof(ApplyDamage), _damageDelay);
        Destroy(gameObject, _lifetime);
    }

    private void ApplyDamage()
    {
        if (_targetUnit == null)
            return;

        _targetUnit.TakeDamage(_damage);
    }
}
