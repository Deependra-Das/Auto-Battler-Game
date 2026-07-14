using AutoBattler.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElementalArrow : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private VisualEffect _vfxParticleGraph;
    [SerializeField] private List<RangedAbilityElementTrailMaterialEntry> _elementalTrailMaterialList;
    private Dictionary<UnitElementEnum, Material> _elementalTrailMaterialDictionary;


    private BaseUnit _ownerUnit;
    private BaseUnit _targetUnit;
    private int _damage;
    private Vector3 _direction;
    private Vector3 _adjustedTargetPosition;
    private UnitElementEnum _element;
    private float _arrowLifetime;
    private float _speed = 7f;
    private float _hitDistance = 0.15f;
    private Coroutine _moveCoroutine;
    private RangedAbilityPoolService _rangedAbilityPoolServiceObj;
    private VfxPoolService _vfxPoolServiceObj;

    private void Awake()
    {
        _elementalTrailMaterialDictionary = new Dictionary<UnitElementEnum, Material>();

        foreach (var entry in _elementalTrailMaterialList)
        {
            _elementalTrailMaterialDictionary[entry.element] = entry.trailMaterial;
        }
    }

    public void Initialize(BaseUnit ownerUnit, BaseUnit targetUnit, int damage, UnitElementEnum attackElement, Vector3 adjustedDirection, Vector3 adjustedTargetPosition, float arrowLifetime, RangedAbilityPoolService rangedAbilityPoolServiceObj)
    {
        if (targetUnit != null)
        {
            _ownerUnit = ownerUnit;
            _targetUnit = targetUnit;
            _damage = damage;
            _element= attackElement;
            _direction = adjustedDirection;
            _adjustedTargetPosition = adjustedTargetPosition;
            _arrowLifetime = arrowLifetime;
            _rangedAbilityPoolServiceObj = rangedAbilityPoolServiceObj;
            _vfxPoolServiceObj = GameManager.Instance.Get<VfxPoolService>();

            RotateTowards(_direction);
            SetTrailMaterial(attackElement);
            SetVfxTrailParticleColor(attackElement);

            _trailRenderer.Clear();
            _trailRenderer.emitting = true;
            _vfxParticleGraph.Reinit();
            _vfxParticleGraph.Play();

            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        float elapsed = 0f;

        while (elapsed < _arrowLifetime)
        {
            if (_targetUnit == null || _targetUnit.IsDead)
            {
                _rangedAbilityPoolServiceObj.DespawnElementalArrow(this);
                yield break;
            }

            transform.position += _direction * _speed * Time.deltaTime;

            Vector3 toTarget = _adjustedTargetPosition - transform.position;

            if (Vector3.Dot(toTarget, _direction) <= 0f || Vector3.Distance(transform.position, _adjustedTargetPosition) <= _hitDistance)
            {
                SpawnElementalVfx(_element, _targetUnit.CurrentNode.worldPosition);
                _targetUnit.TakeDamage(_damage, _element);
                _rangedAbilityPoolServiceObj.DespawnElementalArrow(this);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rangedAbilityPoolServiceObj.DespawnElementalArrow(this);
    }

    private void RotateTowards(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void SetTrailMaterial(UnitElementEnum element)
    {
        _trailRenderer.material = _elementalTrailMaterialDictionary[element];
    }

    private void SetVfxTrailParticleColor(UnitElementEnum element)
    {
        Color color = _elementalTrailMaterialDictionary[element].GetColor("_Color01");
        _vfxParticleGraph.SetVector4("HeadColor", color);
    }

    private void SpawnElementalVfx(UnitElementEnum element, Vector3 position)
    {
        if (_targetUnit == null || _targetUnit.IsDead) return;

        switch (element)
        {
            case UnitElementEnum.Fire:
                AudioManager.Instance.PlayFireAttackAudio();
                _vfxPoolServiceObj.SpawnFireVfx(position);
                break;
            case UnitElementEnum.Nature:
                AudioManager.Instance.PlayNatureAttackAudio();
                _vfxPoolServiceObj.SpawnNatureVfx(position);
                break;
            case UnitElementEnum.Thunder:
                AudioManager.Instance.PlayThunderAttackAudio();
                _vfxPoolServiceObj.SpawnThunderVfx(position);
                break;
        }
    }

    public void Reset()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = null;
        _ownerUnit = null;
        _targetUnit = null;
        _damage = 0;
        _element = default;
        _direction = Vector3.zero;
        _adjustedTargetPosition = Vector3.zero;
        _arrowLifetime = 0;
        _rangedAbilityPoolServiceObj = null;

        _trailRenderer.emitting = false;
        _trailRenderer.Clear();

        _vfxParticleGraph.Stop();
        _vfxParticleGraph.Reinit();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
