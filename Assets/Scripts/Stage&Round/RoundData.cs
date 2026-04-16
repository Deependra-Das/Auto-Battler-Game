using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundData
{
    public int roundIndex;
    public int winXPCurrency;
    public int lossXPCurrency;
    public List<EnemyData> enemiyList;
}