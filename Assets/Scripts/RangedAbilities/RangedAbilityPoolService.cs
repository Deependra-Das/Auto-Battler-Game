using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAbilityPoolService
{
    private ElementalArrow _arrowPrefab;
    private ElementalBurst _elementalBurstPrefab;
    private float _elementalBurstLifetime;
    private float _arrowLifetime;
    private float _arrowOffset;

    private Transform _rangedAbilityPoolContainerTransform;

    private readonly Queue<ElementalArrow> _elementalArrowPoolQueue = new();
    private readonly Queue<ElementalBurst> _elementalBurstPoolQueue = new();
    private readonly HashSet<ElementalArrow> _activeElementalArrows = new();
    private readonly HashSet<ElementalBurst> _activeElementalBursts = new();

    public RangedAbilityPoolService(RangedAbilitiesScriptableObjectScript rangedAbilities_SO, Transform rangedAbilityPoolContainerTransform)
    {
        _arrowPrefab = rangedAbilities_SO.arrowPrefab;
        _elementalBurstPrefab = rangedAbilities_SO.elementalBurstPrefab;
        _arrowLifetime = rangedAbilities_SO.arrowLifetime;
        _elementalBurstLifetime = rangedAbilities_SO.elementalBurstLifetime;
        _arrowOffset = rangedAbilities_SO.arrowOffset;
        _rangedAbilityPoolContainerTransform = rangedAbilityPoolContainerTransform;
    }

    public void SpawnElementalArrow(BaseUnit owner, BaseUnit target, int damage, UnitElementEnum attackElement)
    {
        if (target == null) return;

        ElementalArrow elementalArrow = null; 

        Vector3 spawnPosition = owner.transform.position;
        Vector3 adjustedTargetPosition = target.transform.position;

        if (owner.DirectionFacing == UnitFacingDirectionEnum.Left || owner.DirectionFacing == UnitFacingDirectionEnum.Right)
        {
            spawnPosition.y += _arrowOffset;
            adjustedTargetPosition.y += _arrowOffset;
        }

        if (_elementalArrowPoolQueue.Count > 0)
        {
            elementalArrow = _elementalArrowPoolQueue.Dequeue();
        }
        else
        {
            elementalArrow = CreateElementalArrow();
        }

        elementalArrow.transform.SetParent(null, false);
        elementalArrow.transform.position = spawnPosition;
        elementalArrow.gameObject.SetActive(true);
        _activeElementalArrows.Add(elementalArrow);
        Vector3 adjustedDirection = (adjustedTargetPosition - spawnPosition);
        elementalArrow.Initialize(owner, target, damage, attackElement, adjustedDirection.normalized, adjustedTargetPosition, _arrowLifetime, this);
    }

    private ElementalArrow CreateElementalArrow()
    {
        ElementalArrow elementalArrow = GameObject.Instantiate(_arrowPrefab);
        elementalArrow.gameObject.SetActive(false);
        return elementalArrow;
    }

    public void DespawnElementalArrow(ElementalArrow elementalArrow)
    {
        if (!_activeElementalArrows.Remove(elementalArrow))
            return;

        elementalArrow.gameObject.SetActive(false);
        elementalArrow.Reset();
        elementalArrow.transform.SetParent(_rangedAbilityPoolContainerTransform, false);
        elementalArrow.transform.localPosition = Vector3.zero;
        _elementalArrowPoolQueue.Enqueue(elementalArrow);
    }

    public void SpawnElementalBurst(BaseUnit owner, BaseUnit target, int damage, UnitElementEnum attackElement, float damageDelay)
    {
        if (target == null) return;

        ElementalBurst elementalBurst = null;

        if (_elementalBurstPoolQueue.Count > 0)
        {
            elementalBurst = _elementalBurstPoolQueue.Dequeue();
        }
        else
        {
            elementalBurst = CreateElementalBurst();
        }

        elementalBurst.transform.SetParent(null, false);
        elementalBurst.transform.position = target.CurrentNode.worldPosition;
        elementalBurst.gameObject.SetActive(true);
        _activeElementalBursts.Add(elementalBurst);
        elementalBurst.Initialize(owner, target, damage, attackElement, _elementalBurstLifetime, this);
    }

    private ElementalBurst CreateElementalBurst()
    {
        ElementalBurst elementalBurst = GameObject.Instantiate(_elementalBurstPrefab);
        elementalBurst.gameObject.SetActive(false);
        return elementalBurst;
    }

    public void DespawnElementalBurst(ElementalBurst elementalBurst)
    {
        if (!_activeElementalBursts.Remove(elementalBurst))
            return;

        elementalBurst.gameObject.SetActive(false);
        elementalBurst.Reset();
        elementalBurst.transform.SetParent(_rangedAbilityPoolContainerTransform, false);
        elementalBurst.transform.localPosition = Vector3.zero;
        _elementalBurstPoolQueue.Enqueue(elementalBurst);
    }

    private void DespawnAllElementalArrows()
    {
        foreach (var arrow in _activeElementalArrows.ToArray())
        {
            DespawnElementalArrow(arrow);
        }
    }

    private void DespawnAllElementalBursts()
    {
        foreach (var burst in _activeElementalBursts.ToArray())
        {
            DespawnElementalBurst(burst);
        }
    }

    public void Reset()
    {
        DespawnAllElementalArrows();
        DespawnAllElementalBursts();
    }
}