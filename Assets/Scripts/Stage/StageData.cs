using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageName;
    public int xpExchangeCost = 1;
    public int xpExchangeValue = 1;
    public int shopRefreshCost = 1;
    public int initialCurrency;
    public int initialPlayerLevel;
    public int maxPlayerLives;
    public StageDifficultyEnum stageDifficulty;
    [Range(1, 3)] public int recommendedLevel = 1;
    public List<UnitElementEnum> recommendedElements;
    public List<RoundData> roundDataList;
}