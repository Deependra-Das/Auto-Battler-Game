using UnityEngine;

public class Arrow : MonoBehaviour
{
    private BaseUnit _ownerUnit;
    private BaseUnit _targetUnit;
    private int _damage;
    private float _speed = 7f;
    private Vector3 _direction;
    private float _hitDistance = 0.15f;
    private Vector3 _adjustedTargetPosition;

    public void Initialize(BaseUnit ownerUnit, BaseUnit targetUnit, int dmg, Vector3 adjustedDirection, Vector3 adjustedTargetPosition)
    {
        if (targetUnit != null)
        {
            _ownerUnit = ownerUnit;
            _targetUnit = targetUnit;
            _damage = dmg;
            _direction = adjustedDirection;
            _adjustedTargetPosition = adjustedTargetPosition;
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

        if (Vector3.Distance(transform.position, _adjustedTargetPosition) <= _hitDistance)
        {
            _targetUnit.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
