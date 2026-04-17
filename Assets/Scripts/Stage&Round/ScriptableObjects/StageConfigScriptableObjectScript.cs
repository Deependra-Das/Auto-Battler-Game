using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageConfigScriptableObjectScript", menuName = "ScriptableObjects/StageConfigScriptableObjectScript")]
public class StageConfigScriptableObjectScript : ScriptableObject
{
    public List<StageData> stages;
}
