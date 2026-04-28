using System.Collections.Generic;
using UnityEngine;

public class RoundSaveStateData
{
    public int stageIndex;
    public int roundIndex;

    public int playerLevel;
    public int playerXP;
    public int playerCurrency;

    public List<UnitData> playerInventoryUnits;

    public RoundResultEnum result;
}