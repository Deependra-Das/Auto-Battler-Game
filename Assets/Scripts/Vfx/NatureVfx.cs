using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class NatureVfx : MonoBehaviour
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

        _despawnCoroutine = StartCoroutine(DespawnNatureVfx(vFXPoolServiceObj));
    }

    IEnumerator DespawnNatureVfx(VfxPoolService vFXPoolServiceObj)
    {
        yield return new WaitForSeconds(_lifetime);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnNatureVfx(this);
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
