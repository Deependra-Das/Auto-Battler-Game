using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoInstructionScriptableObjectScript", menuName = "ScriptableObjects/VideoInstructionScriptableObjectScript")]
public class VideoInstructionScriptableObjectScript : ScriptableObject
{
    public List<VideoInstructionEntry> videoInstructionList;
}
