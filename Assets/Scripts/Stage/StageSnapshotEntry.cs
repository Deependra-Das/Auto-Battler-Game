using System;

[Serializable]
public class StageSnapshotEntry
{
    public int stageIndex;

    public int winCount;
    public int drawCount;
    public int loseCount;

    public RoundSnapshotData latestRoundSnapshot;
}
