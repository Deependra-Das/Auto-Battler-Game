using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VfxPoolService
{
    private SmokeVfx _smokeVfxPrefab;
    private HealingVfx _healingVfxPrefab;
    private Transform _vfxPoolContainerTransform;
    private readonly Queue<SmokeVfx> _smokeVfxPoolQueue = new();
    private readonly Queue<HealingVfx> _healingVfxPoolQueue = new();

    public VfxPoolService(VfxScriptableObjectScript vfx_SO, Transform vfxPoolContainerTransform)
    {
        _smokeVfxPrefab = vfx_SO.smokeVfxPrefab;
        _healingVfxPrefab = vfx_SO.healingVfxPrefab;
        _vfxPoolContainerTransform = vfxPoolContainerTransform;
    }

    public void SpawnSmokeVfx(Vector3 position)
    {
        SmokeVfx smokeEffectVfx = null;

        if (_smokeVfxPoolQueue.Count > 0)
        {
            smokeEffectVfx = _smokeVfxPoolQueue.Dequeue();
        }
        else
        {
            smokeEffectVfx = CreateSmokeVfx();
        }

        smokeEffectVfx.transform.SetParent(null, false);
        smokeEffectVfx.transform.position = position;
        smokeEffectVfx.gameObject.SetActive(true);
        smokeEffectVfx.Play(this);
    }

    private SmokeVfx CreateSmokeVfx()
    {
        SmokeVfx smokeEffectVfx = Object.Instantiate(_smokeVfxPrefab);
        smokeEffectVfx.gameObject.SetActive(false);
        return smokeEffectVfx;
    }

    public void DespawnSmokeVfx(SmokeVfx smokeVfx)
    {
        smokeVfx.gameObject.SetActive(false);
        smokeVfx.Reset();
        smokeVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        smokeVfx.transform.localPosition = Vector3.zero;
        _smokeVfxPoolQueue.Enqueue(smokeVfx);
    }


    public void SpawnHealingVfx(Vector3 position)
    {
        HealingVfx healingEffectVfx = null;

        if (_smokeVfxPoolQueue.Count > 0)
        {
            healingEffectVfx = _healingVfxPoolQueue.Dequeue();
        }
        else
        {
            healingEffectVfx = CreateHealingVfx();
        }

        healingEffectVfx.transform.SetParent(null, false);
        healingEffectVfx.transform.position = position;
        healingEffectVfx.gameObject.SetActive(true);
        healingEffectVfx.Play(this);
    }

    private HealingVfx CreateHealingVfx()
    {
        HealingVfx healingEffectVfx = Object.Instantiate(_healingVfxPrefab);
        healingEffectVfx.gameObject.SetActive(false);
        return healingEffectVfx;
    }

    public void DespawnHealingVfx(HealingVfx healingEffectVfx)
    {
        healingEffectVfx.gameObject.SetActive(false);
        healingEffectVfx.Reset();
        healingEffectVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        healingEffectVfx.transform.localPosition = Vector3.zero;
        _healingVfxPoolQueue.Enqueue(healingEffectVfx);
    }


}
