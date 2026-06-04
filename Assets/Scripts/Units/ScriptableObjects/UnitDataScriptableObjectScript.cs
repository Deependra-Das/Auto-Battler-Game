using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataScriptableObjectScript", menuName = "ScriptableObjects/UnitDataScriptableObjectScript")]
public class UnitDataScriptableObjectScript : ScriptableObject
{
    public List<UnitData> unitDataList;
}
