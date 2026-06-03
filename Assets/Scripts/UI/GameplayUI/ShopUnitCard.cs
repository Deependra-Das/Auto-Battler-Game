using AutoBattler.Main;
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
    [SerializeField] private TMP_Text _unitLevel;

    public UnitData unitData;
    private bool _isInitialized;

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
        _isInitialized = true;
        this.unitData = unitData;
        _btnShopUnitCard.interactable = true;
        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = unitData.unitIcon;
        _unitName.text = unitData.unitName.ToString();
        _unitCost.text = unitData.unitCost.ToString();
    }

    private void OnShopUnitCardClicked()
    {
        GameManager.Instance.Get<ShopService>().BuyUnit(this);
    }

    public void Reset()
    {
        _isInitialized = false;
        _unitIcon.sprite = null;
        _unitFaction.sprite = null;
        _unitType.sprite = null;
        _unitName.text = string.Empty;
        _unitCost.text = string.Empty;
        _unitLevel.text = string.Empty;
        _btnShopUnitCard.interactable = false;
    }
}
