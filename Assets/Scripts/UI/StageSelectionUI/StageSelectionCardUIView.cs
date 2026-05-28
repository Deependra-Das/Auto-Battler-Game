using AutoBattler.Event;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionCardUIView : MonoBehaviour
{
    public int StageIndex { get; private set; }
    public int NumberOfRounds { get; private set; }
    public int RecommendedLevel { get; private set; }
    public StageDifficultyEnum StageDifficulty { get; private set; }
    public List<UnitElementEnum> RecommendedElements { get; private set; }

    [SerializeField] private GameObject _selectedHighlight;
    [SerializeField] private TMP_Text _stageNameText;
    [SerializeField] private TMP_Text _numberOfRoundsText;
    [SerializeField] private Image _stageClearedImage;
    [SerializeField] private Button _stageButton;

    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    private void Awake()
    {
        SetStageCardUISelectedHighlight(false);
    }

    void SubscribeToEvents()
    {
        _stageButton.onClick.AddListener(OnStageButtonClicked);
    }

    void UnsubscribeToEvents()
    {
        _stageButton.onClick.RemoveListener(OnStageButtonClicked);
    }

    public void Initialize(int index, StageData stageData)
    {
        StageIndex = index;
        _stageNameText.text = stageData.stageName;
        NumberOfRounds = stageData.roundDataList.Count;
        RecommendedLevel = stageData.recommendedLevel;
        StageDifficulty = stageData.stageDifficulty;
        RecommendedElements = stageData.recommendedElements;
    }

    private void OnStageButtonClicked()
    {
        EventBusManager.Instance.Raise(EventNameEnum.SelectedStageChanged, StageIndex);
    }

    public void SetStageCardUISelectedHighlight(bool value)
    {
        _selectedHighlight.SetActive(value);
    }

    public void SetStageRoundData(int roundsCleared)
    {
        _numberOfRoundsText.text = roundsCleared.ToString() + " / " + NumberOfRounds.ToString();
        _stageClearedImage.gameObject.SetActive((roundsCleared == NumberOfRounds));
    }
}