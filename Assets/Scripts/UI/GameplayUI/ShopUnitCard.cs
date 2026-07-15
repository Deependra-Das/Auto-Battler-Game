using AutoBattler.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUnitCard : MonoBehaviour
{
    [SerializeField] private Button _btnShopUnitCard;
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFactionIcon;
    [SerializeField] private Image _unitTypeIcon;
    [SerializeField] private Image _unitElementIcon;
    [SerializeField] private Image _unitElementIconContainer;
    [SerializeField] private Image _overlay;
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _unitCostText;
    [SerializeField] private TMP_Text _unitLevelText;

    public UnitData UnitData { get; private set; }
    private bool _isInitialized;
    private IconService _unitIconServiceObj;
    private UnitColorService _unitColorServiceObj;

    private void Awake() => SubscribeToEvents();

    private void OnDestroy() => UnsubscribeToEvents();
   
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
        _unitIconServiceObj = GameManager.Instance.Get<IconService>();
        _unitColorServiceObj = GameManager.Instance.Get<UnitColorService>();
        _isInitialized = true;
        UnitData = unitData;
        _btnShopUnitCard.interactable = true;
        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = UnitData.unitIcon;
        _unitNameText.text = UnitData.unitName.ToString();
        _unitCostText.text = UnitData.baseUnitCost.ToString();
        _unitLevelText.text = UnitData.unitLevel.ToString();
        _unitFactionIcon.sprite = _unitIconServiceObj.GetFactionIcon(UnitData.unitFaction);
        _unitTypeIcon.sprite = _unitIconServiceObj.GetUnitTypeIcon(UnitData.unitType);
        _unitElementIcon.sprite = _unitIconServiceObj.GetElementIcon(UnitData.unitElement);
        Color elementColor  = _unitColorServiceObj.GetElementColor(UnitData.unitElement);
        _unitElementIconContainer.color = elementColor;
        _overlay.color = elementColor;
    }

    private void OnShopUnitCardClicked()
    {
        if (!_isInitialized) return;
        GameManager.Instance.Get<ShopService>().BuyUnit(this);
    }

    public void Reset()
    {
        UnitData = default;
        _isInitialized = false;
        _unitIcon.sprite = null;
        _unitFactionIcon.sprite = null;
        _unitTypeIcon.sprite = null;
        _unitElementIcon.sprite = null;
        _unitNameText.text = string.Empty;
        _unitCostText.text = string.Empty;
        _unitLevelText.text = string.Empty;
        _btnShopUnitCard.interactable = false;
    }
}
