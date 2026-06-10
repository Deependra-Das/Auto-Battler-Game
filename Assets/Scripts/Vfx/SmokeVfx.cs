using System.Collections;
using UnityEngine;

public class SmokeVfx : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _duration = 1.2f;

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
        yield return null;
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(stateInfo.length);
        yield return new WaitForSeconds(stateInfo.length);
        _despawnCoroutine = null;
        vFXPoolServiceObj.DespawnSmokeEffectVFX(this);
    }

    private void OnDisable()
    {
        _despawnCoroutine = null;
        _animator.ResetTrigger("PlaySmoke");
    }
}
