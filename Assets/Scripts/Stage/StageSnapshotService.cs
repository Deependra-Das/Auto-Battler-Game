using AutoBattler.Main;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StageSnapshotService
{
    private RoundSnapshotService _roundSnapshotDataObj;

    private const string SAVE_FILE_NAME = "stage_snapshot_data.json";
    private const string SAVE_FOLDER = "SaveData";

    private string SavePath => System.IO.Path.Combine(Application.persistentDataPath, SAVE_FOLDER, SAVE_FILE_NAME);

    public StageSnapshotService()
    {
        _roundSnapshotDataObj = GameManager.Instance.Get<RoundSnapshotService>();
        EnsureFileExists();
    }

    private void EnsureFileExists()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                var initialData = new StageSnapshotData
                {
                    stageSnapshotList = new List<RoundSnapshotData>()
                };

                Write(initialData);

                Debug.Log("Stage snapshot file created at: " + SavePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create snapshot file: {ex}");
        }
    }

    public void SaveStageSnapshotData()
    {
        RoundSnapshotData data = _roundSnapshotDataObj.GetLastSavedRoundSnapshotData();

        try
        {
            StageSnapshotData snapShotData = LoadStageSnapShotData() ?? new StageSnapshotData();

            snapShotData.stageSnapshotList.RemoveAll(s => s.stageIndex == data.stageIndex);
            snapShotData.stageSnapshotList.Add(data);

            Write(snapShotData);

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
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public RoundSnapshotData GetStageSnapshot(int stageIndex)
    {
        var stageSnapShotData = LoadStageSnapShotData();

        if (stageSnapShotData == null)
            return null;

        return stageSnapShotData.stageSnapshotList.Find(s => s.stageIndex == stageIndex);
    }

    public bool HasStageSnapshot(int stageIndex)
    {
        var stageSnapShotData = LoadStageSnapShotData();

        if (stageSnapShotData == null)
            return false;

        return stageSnapShotData.stageSnapshotList.Exists(s => s.stageIndex == stageIndex);
    }

    public void DeleteStageSnapshot(int stageIndex)
    {
        var stageSnapShotData = LoadStageSnapShotData();

        if (stageSnapShotData == null)
            return;

        stageSnapShotData.stageSnapshotList.RemoveAll(s => s.stageIndex == stageIndex);

        Write(stageSnapShotData);
    }

    public void DeleteStageSnapshotDataFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}