using AutoBattler.Main;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class StageSnapshotService
{
    private RoundSnapshotService _roundSnapshotDataObj;

    private const string SAVE_FILE_NAME = "stage_snapshot_data.json";
    private const string SAVE_FOLDER = "SaveData";

    private string SavePath =>
        System.IO.Path.Combine(Application.persistentDataPath, SAVE_FOLDER, SAVE_FILE_NAME);

    public StageSnapshotService()
    {
        _roundSnapshotDataObj = GameManager.Instance.Get<RoundSnapshotService>();
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
            RoundSnapshotData data = _roundSnapshotDataObj.GetLastSavedRoundSnapshotData();

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

            switch (data.result)
            {
                case RoundResultEnum.Win:
                    stageEntry.winCount++;
                    break;

                case RoundResultEnum.Draw:
                    stageEntry.drawCount++;
                    break;

                case RoundResultEnum.Lose:
                    stageEntry.loseCount++;
                    break;
            }

            stageEntry.latestRoundSnapshot = data;
            Write(snapshotData);
            Debug.Log($"Snapshot saved: Stage {data.stageIndex}, Round {data.roundIndex}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Snapshot saving failed: {ex}");
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
}