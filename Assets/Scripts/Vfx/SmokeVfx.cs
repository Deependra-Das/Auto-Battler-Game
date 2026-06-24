using System.Collections;
using UnityEngine;

public class SmokeVfx : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _lifetime = 1f;

    private Coroutine _despawnCoroutine;

    public void Play(VfxPoolService vFXPoolServiceObj)
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
        }

        if (_animator != null)
        {
            _animator.SetTrigger("PlaySmoke");
        }

        _despawnCoroutine = StartCoroutine(DespawnSmokeVfx(vFXPoolServiceObj));
    }

    IEnumerator DespawnSmokeVfx(VfxPoolService vFXPoolServiceObj)
    {
        yield return new WaitForSeconds(_lifetime);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnSmokeVfx(this);
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
