using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAbilitiesScriptableObjectScript", menuName = "ScriptableObjects/RangedAbilitiesScriptableObjectScript")]
public class RangedAbilitiesScriptableObjectScript : ScriptableObject
{
    public ElementalArrow arrowPrefab;
    public float arrowLifetime = 2f;
    public float arrowOffset = 0.35f;
    public ElementalBurst elementalBurstPrefab;
    public float elementalBurstLifetime = 0.5f;
}
