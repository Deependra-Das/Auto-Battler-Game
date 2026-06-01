using AutoBattler.Main;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class StageSnapshotService
{
    private RoundSnapshotService _roundSnapshotServiceObj;

    private const string SAVE_FILE_NAME = "stage_snapshot_data.json";
    private const string SAVE_FOLDER = "SaveData";

    private string SavePath =>
        System.IO.Path.Combine(Application.persistentDataPath, SAVE_FOLDER, SAVE_FILE_NAME);

    public StageSnapshotService()
    {
        _roundSnapshotServiceObj = GameManager.Instance.Get<RoundSnapshotService>();
        EnsureDirectoryExists();
        EnsureFileExists();
    }

    private void EnsureDirectoryExists()
    {
        string directory = System.IO.Path.GetDirectoryName(SavePath);

        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
    }

    private void EnsureFileExists()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                var initialData = new StageSnapshotData
                {
                    stageSnapshotList = new List<StageSnapshotEntry>()
                };

                Write(initialData);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create snapshot file: {ex}");
        }
    }

    public void SaveStageSnapshotData()
    {
        try
        {
            RoundSnapshotData data = _roundSnapshotServiceObj.GetRoundEndSnapshot();

            StageSnapshotData snapshotData = LoadStageSnapShotData() ?? new StageSnapshotData();

            snapshotData.stageSnapshotList ??= new List<StageSnapshotEntry>();

            StageSnapshotEntry stageEntry = snapshotData.stageSnapshotList.Find(s => s.stageIndex == data.stageIndex);

            if (stageEntry == null)
            {
                stageEntry = new StageSnapshotEntry
                {
                    stageIndex = data.stageIndex
                };

                snapshotData.stageSnapshotList.Add(stageEntry);
            }

            SetRoundResult(stageEntry, data.roundIndex, data.result);
            RecalculateCounts(stageEntry);
            stageEntry.latestRoundSnapshot = data;
            Write(snapshotData);
            Debug.Log($"Snapshot saved: Stage {data.stageIndex}, Round {data.roundIndex}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Snapshot saving failed: {ex}");
        }
    }

    private void SetRoundResult(StageSnapshotEntry stageEntry, int roundIndex, RoundResultEnum result)
    {
        stageEntry.roundResults ??= new List<RoundResultEnum>();

        while (stageEntry.roundResults.Count <= roundIndex)
        {
            stageEntry.roundResults.Add(RoundResultEnum.None);
        }

        stageEntry.roundResults[roundIndex] = result;
    }

    private void RecalculateCounts(StageSnapshotEntry entry)
    {
        entry.winCount = 0;
        entry.drawCount = 0;
        entry.loseCount = 0;

        foreach (var result in entry.roundResults)
        {
            switch (result)
            {
                case RoundResultEnum.Win:
                    entry.winCount++;
                    break;

                case RoundResultEnum.Draw:
                    entry.drawCount++;
                    break;

                case RoundResultEnum.Lose:
                    entry.loseCount++;
                    break;
            }
        }
    }

    public StageSnapshotData LoadStageSnapShotData()
    {
        try
        {
            if (!File.Exists(SavePath))
                return null;

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<StageSnapshotData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Load failed: {ex}");
            return null;
        }
    }

    private void Write(StageSnapshotData data)
    {
        EnsureDirectoryExists();

        string json = JsonUtility.ToJson(data, true);
        string tempPath = SavePath + ".tmp";

        File.WriteAllText(tempPath, json);

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.Move(tempPath, SavePath);
    }

    public StageSnapshotEntry GetStageSnapshot(int stageIndex)
    {
        var data = LoadStageSnapShotData();

        if (data == null)
            return null;

        return data.stageSnapshotList.Find(s => s.stageIndex == stageIndex);
    }

    public bool HasStageSnapshot(int stageIndex)
    {
        var data = LoadStageSnapShotData();

        if (data == null)
            return false;

        return data.stageSnapshotList
            .Exists(s => s.stageIndex == stageIndex);
    }

    public void DeleteStageSnapshot(int stageIndex)
    {
        var data = LoadStageSnapShotData();

        if (data == null)
            return;

        data.stageSnapshotList.RemoveAll(s => s.stageIndex == stageIndex);

        Write(data);
    }

    public void DeleteStageSnapshotDataFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        EnsureFileExists();
    }

    public void Dispose()
    {
        _roundSnapshotServiceObj = null;
    }
}