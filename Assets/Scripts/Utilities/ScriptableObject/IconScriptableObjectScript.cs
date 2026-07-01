using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconScriptableObjectScript", menuName = "ScriptableObjects/IconScriptableObjectScript")]
public class IconScriptableObjectScript : ScriptableObject
{
    public List<UnitFactionIconEntry> factionIconList;
    public List<UnitElementIconEntry> elementIconList;
    public List<UnitTypeIconEntry> unitTypeIconList;
    public List<BuffIconEntry> buffIconList;
    public List<UnitIconEntry> unitIconList;
}
