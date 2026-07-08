using AutoBattler.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : GenericMonoSingleton<PostProcessingManager>
{
    [SerializeField] private ScriptableRendererFeature _fullscreenVornoiEffect;
    [SerializeField] private Volume volume;

    private DepthOfField _depthOfFieldObj;

    protected override void Awake()
    {
        base.Awake();
        volume.profile.TryGet(out _depthOfFieldObj);

    }

    public void ToggleBlur(bool value)
    {
        _depthOfFieldObj.active = value;
    }

    public void ToggleFullscreenVornoiEffect(bool value)
    {
        _fullscreenVornoiEffect.SetActive(value);
    }
}
