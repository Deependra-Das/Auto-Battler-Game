using UnityEngine;

public class Arrow : MonoBehaviour
{
    private BaseUnit _targetUnit;
    private int _damage;
    private float _speed = 7f;
    private Vector3 _direction;
    private float _hitDistance = 0.15f;

    public void Initialize(BaseUnit targetUnit, int dmg)
    {
        _targetUnit = targetUnit;
        _damage = dmg;

        if (_targetUnit != null)
        {
            _direction = (_targetUnit.transform.position - transform.position).normalized;
            RotateTowards(_direction);
        }
    }

    private void Update()
    {
        if (_targetUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += _direction * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _targetUnit.transform.position) <= _hitDistance)
        {
            _targetUnit.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }

    private void RotateTowards(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
