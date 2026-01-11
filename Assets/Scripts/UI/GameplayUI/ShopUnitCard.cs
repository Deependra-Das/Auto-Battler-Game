using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUnitCard : MonoBehaviour
{
    [SerializeField] private Button _btnShopUnitCard;
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFaction;
    [SerializeField] private Image _unitType;
    [SerializeField] private TMP_Text _unitName;
    [SerializeField] private TMP_Text _unitCost;
    [SerializeField] private Image _unitLevel;

    private UnitData _unitData;

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();
   
    private void SubscribeToEvents()
    {
        _btnShopUnitCard.onClick.AddListener(OnShopUnitCardClicked);
    }

    private void UnsubscribeToEvents()
    {
        _btnShopUnitCard.onClick.RemoveListener(OnShopUnitCardClicked);
    }

    public void Initialize(UnitData unitData)
    {
        _unitData = unitData;
        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = _unitData.unitIcon;
        //_unitFaction.sprite = ;
        //_unitType.sprite = ;
        _unitName.text = _unitData.unitName.ToString();
        _unitCost.text = _unitData.unitCost.ToString();
        //_unitLevel.sprite = ;
    }

    private void OnShopUnitCardClicked()
    {
        Debug.Log(_unitData.unitID);
    }
}
