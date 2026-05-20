using AutoBattler.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionCardUIView : MonoBehaviour
{
    public int StageIndex { get; private set; }

    [SerializeField] private GameObject _selectedHighlight;
    [SerializeField] private TMP_Text _stageNameText;
    [SerializeField] private TMP_Text _numberOfRoundsText;
    [SerializeField] private Image _stageClearedImage;
    [SerializeField] private Button _stageButton;

    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    private int _numberOfRounds = 0; 

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

    public void Initialize(int stageIndex, string stageName, int numberOfRounds)
    {
        StageIndex = stageIndex;
        _stageNameText.text = stageName;
        _numberOfRounds = numberOfRounds;
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
        _numberOfRoundsText.text = roundsCleared.ToString() + " / " + _numberOfRounds.ToString();
        _stageClearedImage.gameObject.SetActive((roundsCleared == _numberOfRounds));
    }
}