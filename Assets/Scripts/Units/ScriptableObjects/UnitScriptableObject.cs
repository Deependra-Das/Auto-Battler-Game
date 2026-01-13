using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitScriptableObject", menuName = "ScriptableObjects/UnitScriptableObject")]
public class UnitScriptableObject : ScriptableObject
{
    public List<UnitData> unitDataList;

    private void OnValidate()
    {
        if (unitDataList == null)
            return;

        for (int i = 0; i < unitDataList.Count; i++)
        {
            UnitData data = unitDataList[i];
            data.unitID = i;
            unitDataList[i] = data;
        }
    }
}
