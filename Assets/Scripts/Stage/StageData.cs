using System.Collections.Generic;

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
    public List<RoundData> roundDataList;
}