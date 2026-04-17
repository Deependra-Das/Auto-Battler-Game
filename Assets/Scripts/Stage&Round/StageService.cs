using UnityEngine;

public class StageService
{
    private StageConfigScriptableObjectScript _stageConfig;
    private PlayerLevelService _playerLevelConfig;

    public int CurrentStage { get; private set; }
    public int CurrentRound { get; private set; }

    public StageService(StageConfigScriptableObjectScript config)
    {
        _stageConfig = config;
    }

    public void StartStage(int stageIndex)
    {
        CurrentStage = stageIndex;
        CurrentRound = 0;
        Debug.Log($"Starting Stage {CurrentStage}");
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Starting Stage {CurrentStage} - Round {CurrentRound}");
    }

    public void OnRoundWin()
    {
        var round = GetCurrentRoundData();
        AdvanceRound();
    }

    public void OnRoundLose()
    {
        var round = GetCurrentRoundData();

        if (_playerLevelConfig.IsPlayerDead())
        {
            OnStageFailed();
            return;
        }

        AdvanceRound();
    }

    private void AdvanceRound()
    {
        CurrentRound++;

        if (CurrentRound >= 5)
        {
            OnStageCleared();
        }
        else
        {
            StartRound();
        }
    }

    private void OnStageCleared()
    {
        Debug.Log("Stage Cleared!");

        if (CurrentStage >= _stageConfig.stages.Count - 1)
        {
            OnGameCompleted();
            return;
        }

        StartStage(CurrentStage + 1);
    }

    private void OnGameCompleted()
    {
        Debug.Log("Game Completed!");
    }

    private void OnStageFailed()
    {
        Debug.Log("Stage Failed. Restarting...");
        StartStage(CurrentStage);
    }

    public StageData GetCurrentStageData()
    {
        return _stageConfig.stages[CurrentStage];
    }

    public RoundData GetCurrentRoundData()
    {
        return GetCurrentStageData().roundDataList[CurrentRound];
    }
}
