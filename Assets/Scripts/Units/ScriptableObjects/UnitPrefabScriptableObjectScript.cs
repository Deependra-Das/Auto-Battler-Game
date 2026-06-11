using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPrefabScriptableObjectScript", menuName = "ScriptableObjects/UnitPrefabScriptableObjectScript")]
public class UnitPrefabScriptableObjectScript : ScriptableObject
{
    public List<UnitPrefabEntry> unitPrefabList;
}
