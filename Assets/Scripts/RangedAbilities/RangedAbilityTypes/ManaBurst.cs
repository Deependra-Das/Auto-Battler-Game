using UnityEngine;

public class ManaBurst : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime = 1f;
    private float _damageDelay = 0.2f;

    public void Initialize(BaseUnit targetUnit, int damage)
    {
        _targetUnit = targetUnit;
        _damage = damage;

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
