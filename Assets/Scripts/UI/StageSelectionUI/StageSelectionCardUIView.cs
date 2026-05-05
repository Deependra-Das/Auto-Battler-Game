using AutoBattler.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionCardUIView : MonoBehaviour
{
    private int _stageIndex;
    [SerializeField] private TMP_Text _stageNameText;
    [SerializeField] private TMP_Text _numberOfRoundsText;
    [SerializeField] private Button _stageButton;

    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

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
        _stageIndex = stageIndex;
        _numberOfRoundsText.text = stageData.roundDataList.Count.ToString();
    }

    private void OnStageButtonClicked()
    {
        Debug.Log(_stageIndex);
        GameData.selectedStage = _stageIndex;
        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene);
    }
}