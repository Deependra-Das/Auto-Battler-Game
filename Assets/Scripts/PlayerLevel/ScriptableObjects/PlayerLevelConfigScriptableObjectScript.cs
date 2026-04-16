using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLevelConfigScriptableObjectScript", menuName = "ScriptableObjects/PlayerLevelConfigScriptableObjectScript")]
public class PlayerLevelConfigScriptableObjectScript : ScriptableObject
{
    public List<PlayerLevelData> playerProgressionDataList;
}
