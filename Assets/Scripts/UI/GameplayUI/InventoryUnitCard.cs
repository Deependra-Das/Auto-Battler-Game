using AutoBattler.Main;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUnitCard : MonoBehaviour
{
    [SerializeField] private Button _btnInventoryUnitCard;
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFaction;
    [SerializeField] private Image _unitType;
    [SerializeField] private Image _unitLevel;

    public UnitData unitData;

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        _btnInventoryUnitCard.onClick.AddListener(OnInventoryUnitCardClicked);
    }

    private void UnsubscribeToEvents()
    {
        _btnInventoryUnitCard.onClick.RemoveListener(OnInventoryUnitCardClicked);
    }

    public void Initialize(UnitData unitData)
    {
        this.unitData = unitData;
        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = unitData.unitIcon;
        //_unitFaction.sprite = ;
        //_unitType.sprite = ;
        //_unitLevel.sprite = ;
    }

    private void OnInventoryUnitCardClicked()
    {
        GameManager.Instance.Get<InventoryService>().DeployUnit(this);
    }
}
