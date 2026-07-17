using System.Collections.Generic;
using UnityEngine;

public class VfxPoolService
{
    private SmokeVfx _smokeVfxPrefab;
    private HealingVfx _healingVfxPrefab;
    private FireVfx _fireVfxPrefab;
    private NatureVfx _natureVfxPrefab;
    private ThunderVfx _thunderVfxPrefab;
    private Transform _vfxPoolContainerTransform;

    private readonly Queue<SmokeVfx> _smokeVfxPoolQueue = new();
    private readonly Queue<HealingVfx> _healingVfxPoolQueue = new();
    private readonly Queue<FireVfx> _fireVfxPoolQueue = new();
    private readonly Queue<NatureVfx> _natureVfxPoolQueue = new();
    private readonly Queue<ThunderVfx> _thunderVfxPoolQueue = new();

    Vector3 topScreenWorld = Vector3.zero;

    public VfxPoolService(VfxScriptableObjectScript vfx_SO, Transform vfxPoolContainerTransform)
    {
        _smokeVfxPrefab = vfx_SO.smokeVfxPrefab;
        _healingVfxPrefab = vfx_SO.healingVfxPrefab;
        _fireVfxPrefab = vfx_SO.fireVfxPrefab;
        _natureVfxPrefab = vfx_SO.natureVfxPrefab;
        _thunderVfxPrefab = vfx_SO.thunderVfxPrefab;
        _vfxPoolContainerTransform = vfxPoolContainerTransform;

        topScreenWorld = Camera.main.ScreenToWorldPoint(new Vector3(
          Screen.width / 2f, Screen.height, Mathf.Abs(Camera.main.transform.position.z)));
    }

    public void SpawnSmokeVfx(Vector3 spawnPosition)
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
        smokeEffectVfx.transform.position = spawnPosition;
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

        if (_healingVfxPoolQueue.Count > 0)
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

    public void SpawnFireVfx(Vector3 spawnPosition)
    {
        FireVfx fireEffectVfx = null;

        if (_fireVfxPoolQueue.Count > 0)
        {
            fireEffectVfx = _fireVfxPoolQueue.Dequeue();
        }
        else
        {
            fireEffectVfx = CreateFireVfx();
        }

        fireEffectVfx.transform.SetParent(null, false);
        fireEffectVfx.transform.position = spawnPosition;
        fireEffectVfx.gameObject.SetActive(true);
        fireEffectVfx.Play(this);
    }

    private FireVfx CreateFireVfx()
    {
        FireVfx fireEffectVfx = Object.Instantiate(_fireVfxPrefab);
        fireEffectVfx.gameObject.SetActive(false);
        return fireEffectVfx;
    }

    public void DespawnFireVfx(FireVfx fireEffectVfx)
    {
        fireEffectVfx.gameObject.SetActive(false);
        fireEffectVfx.Reset();
        fireEffectVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        fireEffectVfx.transform.localPosition = Vector3.zero;
        _fireVfxPoolQueue.Enqueue(fireEffectVfx);
    }

    public void SpawnNatureVfx(Vector3 spawnPosition)
    {
        NatureVfx natureEffectVfx = null;

        if (_natureVfxPoolQueue.Count > 0)
        {
            natureEffectVfx = _natureVfxPoolQueue.Dequeue();
        }
        else
        {
            natureEffectVfx = CreateNatureVfx();
        }

        natureEffectVfx.transform.SetParent(null, false);
        natureEffectVfx.transform.position = spawnPosition;
        natureEffectVfx.gameObject.SetActive(true);
        natureEffectVfx.Play(this);
    }

    private NatureVfx CreateNatureVfx()
    {
        NatureVfx natureEffectVfx = Object.Instantiate(_natureVfxPrefab);
        natureEffectVfx.gameObject.SetActive(false);
        return natureEffectVfx;
    }

    public void DespawnNatureVfx(NatureVfx natureEffectVfx)
    {
        natureEffectVfx.gameObject.SetActive(false);
        natureEffectVfx.Reset();
        natureEffectVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        natureEffectVfx.transform.localPosition = Vector3.zero;
        _natureVfxPoolQueue.Enqueue(natureEffectVfx);
    }

    public void SpawnThunderVfx(Vector3 spawnPosition)
    {
        ThunderVfx thunderEffectVfx = null;

        if (_thunderVfxPoolQueue.Count > 0)
        {
            thunderEffectVfx = _thunderVfxPoolQueue.Dequeue();
        }
        else
        {
            thunderEffectVfx = CreateThunderVfx();
        }

        thunderEffectVfx.transform.SetParent(null, false);
        thunderEffectVfx.transform.position = new Vector3(spawnPosition.x, topScreenWorld.y, spawnPosition.z);

        float _thunderVfxOffsetY = thunderEffectVfx.transform.position.y - spawnPosition.y;

        thunderEffectVfx.gameObject.SetActive(true);
        thunderEffectVfx.Play(this, _thunderVfxOffsetY);
    }

    private ThunderVfx CreateThunderVfx()
    {
        ThunderVfx thunderEffectVfx = Object.Instantiate(_thunderVfxPrefab);
        thunderEffectVfx.gameObject.SetActive(false);
        return thunderEffectVfx;
    }

    public void DespawnThunderVfx(ThunderVfx thunderEffectVfx)
    {
        thunderEffectVfx.gameObject.SetActive(false);
        thunderEffectVfx.Reset();
        thunderEffectVfx.transform.SetParent(_vfxPoolContainerTransform, false);
        thunderEffectVfx.transform.localPosition = Vector3.zero;
        _thunderVfxPoolQueue.Enqueue(thunderEffectVfx);
    }
}
