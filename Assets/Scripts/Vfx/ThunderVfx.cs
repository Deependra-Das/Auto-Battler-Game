using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ThunderVfx : MonoBehaviour
{
    [SerializeField] private VisualEffect _vfxParticleGraph;
    [SerializeField] private float _lifetime = 1f;
    private Coroutine _despawnCoroutine;

    public void Play(VfxPoolService vFXPoolServiceObj, float offset)
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
        }

        _vfxParticleGraph.SetFloat("ImpactOffset", offset);
        _vfxParticleGraph.Reinit();
        _vfxParticleGraph.Play();

        _despawnCoroutine = StartCoroutine(DespawnThunderVfx(vFXPoolServiceObj));
    }

    IEnumerator DespawnThunderVfx(VfxPoolService vFXPoolServiceObj)
    {
        yield return new WaitForSeconds(_lifetime);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnThunderVfx(this);
    }

    public void Reset()
    {
        _vfxParticleGraph.Stop();
        _vfxParticleGraph.Reinit();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
