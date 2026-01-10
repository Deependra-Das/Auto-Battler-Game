using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitScriptableObjectScript", menuName = "ScriptableObjects/UnitScriptableObjectScript")]
public class UnitScriptableObjectScript : ScriptableObject
{
    public List<UnitData> unitDataList;
}
