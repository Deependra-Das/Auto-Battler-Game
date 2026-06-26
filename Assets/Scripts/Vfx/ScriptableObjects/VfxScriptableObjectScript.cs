using UnityEngine;

[CreateAssetMenu(fileName = "VfxScriptableObjectScript", menuName = "ScriptableObjects/VfxScriptableObjectScript")]
public class VfxScriptableObjectScript : ScriptableObject
{
    public SmokeVfx smokeVfxPrefab;
    public HealingVfx healingVfxPrefab;
    public FireVfx fireVfxPrefab;
    public NatureVfx natureVfxPrefab;
    public ThunderVfx thunderVfxPrefab;

    public float thunderVfxOffset;
}
