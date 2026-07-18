using AutoBattler.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : GenericMonoSingleton<PostProcessingManager>
{
    [SerializeField] private ScriptableRendererFeature _fullscreenVornoiEffect;
    [SerializeField] private Volume volume;

    private DepthOfField _depthOfFieldObj;
    private Bloom _bloomObj;

    protected override void Awake()
    {
        base.Awake();
        volume.profile.TryGet(out _depthOfFieldObj);
        volume.profile.TryGet(out _bloomObj);
    }

    public void ToggleBlur(bool value)
    {
        if(_depthOfFieldObj == null ) return;
        _depthOfFieldObj.active = value;
    }

    public void ToggleFullscreenVornoiEffect(bool value)
    {
        if (_fullscreenVornoiEffect == null) return;
        _fullscreenVornoiEffect.SetActive(value);
    }

    public void ToggleBloom(bool value)
    {
        if (_bloomObj == null) return;
        _bloomObj.active = value;
    }
}
