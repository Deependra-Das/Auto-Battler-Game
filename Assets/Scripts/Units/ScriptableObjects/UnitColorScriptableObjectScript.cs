using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusEffectColorScriptableObjectScript", menuName = "ScriptableObjects/UnitStatusEffectColorScriptableObjectScript")]
public class UnitColorScriptableObjectScript : ScriptableObject
{
    public List<UnitElementColorEntry> unitElementColorEntryList;
    public Color healthDamageColor;
    public Color shieldDamageColor;
    public Color healingColor;
}
