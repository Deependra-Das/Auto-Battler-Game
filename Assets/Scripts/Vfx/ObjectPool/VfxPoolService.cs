using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VfxPoolService
{
    private SmokeVfx _smokeEffectVfxPrefab;
    private Transform _vfxPoolContainerTransform;
    private readonly Queue<SmokeVfx> _smokeEffectVfxPoolQueue = new();

    public VfxPoolService(VfxScriptableObjectScript vfx_SO, Transform vfxPoolContainerTransform)
    {
        _smokeEffectVfxPrefab = vfx_SO._smokeEffectVfxPrefab;
        _vfxPoolContainerTransform = vfxPoolContainerTransform;
    }

    public void SpawnSmokeEffectVFX(Vector3 position)
    {
        SmokeVfx smokeEffectVfx = null;

        if (_smokeEffectVfxPoolQueue.Count > 0)
        {
            smokeEffectVfx = _smokeEffectVfxPoolQueue.Dequeue();
        }
        else
        {
            smokeEffectVfx = CreateSmokeEffectVFX();
        }

        smokeEffectVfx.transform.SetParent(null, false);
        smokeEffectVfx.transform.position = position;
        smokeEffectVfx.gameObject.SetActive(true);
        smokeEffectVfx.PlaySmokeEffectVfxAnimation(this);
    }

    private SmokeVfx CreateSmokeEffectVFX()
    {
        SmokeVfx smokeEffectVfx = Object.Instantiate(_smokeEffectVfxPrefab);
        smokeEffectVfx.gameObject.SetActive(false);
        return smokeEffectVfx;
    }

    public void DespawnSmokeEffectVFX(SmokeVfx smokeEffectVfx)
    {
        smokeEffectVfx.gameObject.SetActive(false);
        smokeEffectVfx.Reset();
        smokeEffectVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        smokeEffectVfx.transform.localPosition = Vector3.zero;
        _smokeEffectVfxPoolQueue.Enqueue(smokeEffectVfx);
    }
}
