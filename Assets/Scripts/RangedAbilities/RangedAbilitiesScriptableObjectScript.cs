using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAbilitiesScriptableObjectScript", menuName = "ScriptableObjects/RangedAbilitiesScriptableObjectScript")]
public class RangedAbilitiesScriptableObjectScript : ScriptableObject
{
    public Arrow arrowPrefab;
    public ManaBurst manaBurstPrefab;
}
