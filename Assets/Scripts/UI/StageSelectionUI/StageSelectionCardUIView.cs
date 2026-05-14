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

    public void Initialize(int stageIndex, StageData stageData)
    {
        StageIndex = stageIndex;
        _numberOfRoundsText.text = stageData.roundDataList.Count.ToString();
    }

    private void OnStageButtonClicked()
    {
        EventBusManager.Instance.Raise(EventNameEnum.SelectedStageChanged, StageIndex);
    }

    public void SetStageCardUISelectedHighlight(bool value)
    {
        _selectedHighlight.SetActive(value);
    }
}