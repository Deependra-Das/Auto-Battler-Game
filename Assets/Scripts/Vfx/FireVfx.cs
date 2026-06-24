using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FireVfx : MonoBehaviour
{
    [SerializeField] private VisualEffect _vfxParticleGraph;
    [SerializeField] private float _lifetime = 1f;
    private Coroutine _despawnCoroutine;

    public void Play(VfxPoolService vFXPoolServiceObj)
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
        }

        _vfxParticleGraph.Reinit();
        _vfxParticleGraph.Play();

        _despawnCoroutine = StartCoroutine(DespawnFireVfx(vFXPoolServiceObj));
    }

    IEnumerator DespawnFireVfx(VfxPoolService vFXPoolServiceObj)
    {
        yield return new WaitForSeconds(_lifetime);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnFireVfx(this);
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