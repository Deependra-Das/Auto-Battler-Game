using AutoBattler.Main;
using TMPro;
using Unity.VisualScripting;
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

    public UnitData unitData;

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
        this.unitData = unitData;
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
}
