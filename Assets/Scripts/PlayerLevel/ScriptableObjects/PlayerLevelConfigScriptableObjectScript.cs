using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLevelConfigScriptableObjectScript", menuName = "ScriptableObjects/PlayerLevelConfigScriptableObjectScript")]
public class PlayerLevelConfigScriptableObjectScript : ScriptableObject
{
    public List<PlayerLevelData> playerProgressionDataList;
    public int xpExchangeCost  =1;
    public int xpExchangeValue  = 1;
    public int shopRefreshCost = 1;

    private void OnValidate()
    {
        if (playerProgressionDataList == null || playerProgressionDataList.Count == 0)
        {
            Debug.LogWarning("Player progression list is empty!");
            return;
        }

        for (int i = 0; i < playerProgressionDataList.Count; i++)
        {
            var data = playerProgressionDataList[i];

            if (data.xpRequiredToNextLevel < 0)
            {
                Debug.LogError($"Level {i + 1} has negative XP requirement!");
            }
            if (data.maxUnitsAllowed <= 0)
            {
                Debug.LogError($"Level {i + 1} must allow at least 1 unit!");
            }
            if (i == playerProgressionDataList.Count - 1 && data.xpRequiredToNextLevel != 0)
            {
                Debug.LogWarning("Last level should have 0 XP requirement.");
            }
        }

        if (xpExchangeCost  <= 0)
        {
            Debug.LogError("XP buy cost must be greater than 0!");
        }

        if (xpExchangeValue  <= 0)
        {
            Debug.LogError("XP gain amount must be greater than 0!");
        }
    }
}
