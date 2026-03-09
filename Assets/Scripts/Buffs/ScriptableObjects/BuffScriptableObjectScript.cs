using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffScriptableObjectScript", menuName = "ScriptableObjects/BuffScriptableObjectScript")]
public class BuffScriptableObjectScript : ScriptableObject
{
    public List<BuffData> buffData;
}
