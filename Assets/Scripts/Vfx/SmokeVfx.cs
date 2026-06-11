using System.Collections;
using UnityEngine;

public class SmokeVfx : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _lifetime = 1f;

    private Coroutine _despawnCoroutine;

    public void PlaySmokeEffectVfxAnimation(VfxPoolService vFXPoolServiceObj)
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
        }

        if (_animator != null)
        {
            _animator.SetTrigger("PlaySmoke");
        }

        _despawnCoroutine = StartCoroutine(DespawnSmokeEffectVfxAfterAnimation(vFXPoolServiceObj));
    }

    IEnumerator DespawnSmokeEffectVfxAfterAnimation(VfxPoolService vFXPoolServiceObj)
    {
        yield return new WaitForSeconds(_lifetime);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnSmokeEffectVFX(this);
    }

    public void Reset()
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
        }

        _despawnCoroutine = null;
        _animator.ResetTrigger("PlaySmoke");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
