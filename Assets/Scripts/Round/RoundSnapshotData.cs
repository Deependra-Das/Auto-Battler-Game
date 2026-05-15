using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundSnapshotData
{
    public int stageIndex;
    public int roundIndex;

    public int playerLevel;
    public int playerXP;
    public int playerCurrency;
    public int playerLives;

    public List<UnitSnapshotData> playerInventoryUnits;

    public RoundResultEnum result;
}