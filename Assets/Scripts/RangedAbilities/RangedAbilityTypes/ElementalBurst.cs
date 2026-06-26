using AutoBattler.Main;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ElementalBurst : MonoBehaviour
{
    [SerializeField] private VisualEffect _vfxParticleGraph;

    private BaseUnit _ownerUnit;
    private BaseUnit _targetUnit;
    private int _damage;
    private float _lifetime;
    private UnitElementEnum _element;
    private RangedAbilityPoolService _rangedAbilityPoolServiceObj;
    private VfxPoolService _vfxPoolServiceObj;
    protected UnitColorService _unitColorServiceObj;
    private Coroutine _executeCoroutine;

    public void Initialize(BaseUnit ownerUnit, BaseUnit targetUnit, int damage, UnitElementEnum attackElement, float lifetime, RangedAbilityPoolService rangedAbilityPoolServiceObj)
    {
        _ownerUnit = ownerUnit;
        _targetUnit = targetUnit;
        _damage = damage;
        _element = attackElement;
        _lifetime = lifetime;
        _rangedAbilityPoolServiceObj = rangedAbilityPoolServiceObj;
        _vfxPoolServiceObj = GameManager.Instance.Get<VfxPoolService>();
        _unitColorServiceObj = GameManager.Instance.Get<UnitColorService>();

        if (_executeCoroutine != null)
            StopCoroutine(_executeCoroutine);

        Color color = GetBurstColor(attackElement);
        _vfxParticleGraph.SetVector4("BurstColor", new Vector4(color.r, color.g, color.b, color.a));
        _vfxParticleGraph.Reinit();
        _vfxParticleGraph.Play();
        SpawnElementalVfx(attackElement, _targetUnit.CurrentNode.worldPosition);
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

    protected Color GetBurstColor(UnitElementEnum element)
    {
        return _unitColorServiceObj.GetElementColor(element);
    }

    public void Reset()
    {
        if (_executeCoroutine != null)
            StopCoroutine(_executeCoroutine);

        _vfxParticleGraph.Stop();
        _vfxParticleGraph.Reinit();
        _executeCoroutine = null;
        _ownerUnit = null;
        _targetUnit = null;
        _damage = 0;
        _element = default;
        _lifetime = 0;
        _rangedAbilityPoolServiceObj = null;
    }

    private void SpawnElementalVfx(UnitElementEnum element, Vector3 position)
    {
        if (_targetUnit == null || _targetUnit.IsDead) return;

        switch (element)
        {
            case UnitElementEnum.Fire:
                _vfxPoolServiceObj.SpawnFireVfx(position);
                break;
            case UnitElementEnum.Nature:
                _vfxPoolServiceObj.SpawnNatureVfx(position);
                break;
            case UnitElementEnum.Thunder:
                _vfxPoolServiceObj.SpawnThunderVfx(position);
                break;
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}