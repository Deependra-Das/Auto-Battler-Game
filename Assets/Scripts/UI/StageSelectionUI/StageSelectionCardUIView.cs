using AutoBattler.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
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
    [SerializeField] private Image _stageClearedImage;
    [SerializeField] private Button _stageButton;
    [SerializeField] private Transform _roundResultListContainer;
    [SerializeField] private RoundResultCardUIView _roundResultCardPrefab;

    private Dictionary<int, RoundResultCardUIView> _roundResultCardDictionary = new();

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

        CreateRoundImages();
    }

    private void CreateRoundImages()
    {
        for (int index = 0; index < NumberOfRounds; index++)
        {
            RoundResultCardUIView roundResultCard = Instantiate(_roundResultCardPrefab, _roundResultListContainer, false);
            _roundResultCardDictionary[index] = roundResultCard;
            roundResultCard.Initialize(index);
        }
    }

    private void CleanUpRoundImages()
    {
        foreach (var card in _roundResultCardDictionary.Values)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        _roundResultCardDictionary.Clear();
    }

    private void OnStageButtonClicked()
    {
        EventBusManager.Instance.Raise(EventNameEnum.SelectedStageChanged, StageIndex);
    }

    public void SetStageCardUISelectedHighlight(bool value)
    {
        _selectedHighlight.SetActive(value);
    }

    public void SetStageRoundData(List<RoundResultEnum> roundResults,int roundsCleared, int lastRoundPlayed)
    {
        if(roundResults == null)
        {
            _roundResultCardDictionary.Values.ToList().ForEach(card => card.ResetRoundResult());
        }
        else
        {
            for (int index = 0; index < roundResults.Count; index++)
            {
                _roundResultCardDictionary[index].SetRoundResultImage(roundResults[index]);        
            }
        }

        ToggleRoundClearedImage(roundsCleared == NumberOfRounds);
        SetRoundToContinueFrom(lastRoundPlayed);
    }
    
    private void ToggleRoundClearedImage(bool value)
    {
        _stageClearedImage.gameObject.SetActive(value);
    }

    private void SetRoundToContinueFrom(int lastRoundPlayed)
    {
        int index = Mathf.Clamp(lastRoundPlayed + 1, 0, NumberOfRounds - 1);

        _roundResultCardDictionary.Values.ToList().ForEach(card => card.ToggleRoundMarker(false));
        _roundResultCardDictionary[index].ToggleRoundMarker(true);
    }

    public void InvokeClick()
    {
        _stageButton.onClick.Invoke();
    }

    private void OnDestroy()
    {
        CleanUpRoundImages();
    }
}