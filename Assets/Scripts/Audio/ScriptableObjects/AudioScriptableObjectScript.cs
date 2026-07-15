using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioScriptableObjectScript", menuName = "ScriptableObjects/AudioScriptableObjectScript")]
public class AudioScriptableObjectScript : ScriptableObject
{
    public List<AudioData> audioDataList;
}
