using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VfxPoolService
{
    private SmokeVfx _smokeVfxPrefab;
    private Transform _vfxPoolContainerTransform;
    private readonly Queue<SmokeVfx> _smokeVfxPoolQueue = new();

    public VfxPoolService(VfxScriptableObjectScript vfx_SO, Transform vfxPoolContainerTransform)
    {
        _smokeVfxPrefab = vfx_SO.smokeVfxPrefab;
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
}
