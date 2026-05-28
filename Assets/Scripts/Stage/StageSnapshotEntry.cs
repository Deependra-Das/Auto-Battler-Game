using System;
using System.Collections.Generic;

[Serializable]
public class StageSnapshotEntry
{
    public int stageIndex;

    public int winCount;
    public int drawCount;
    public int loseCount;
    public List<RoundResultEnum> roundResults = new();
    public RoundSnapshotData latestRoundSnapshot;
}
