using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitIconScriptableObjectScript", menuName = "ScriptableObjects/UnitIconScriptableObjectScript")]
public class UnitIconScriptableObjectScript : ScriptableObject
{
    public List<UnitFactionIconEntry> factionIconList;
    public List<UnitElementIconEntry> elementIconList;
    public List<UnitTypeIconEntry> unitTypeIconList;
    public List<UnitIconEntry> unitIconList;
}
