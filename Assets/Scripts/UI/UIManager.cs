using AutoBattler.Main;
using AutoBattler.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GenericMonoSingleton<UIManager>
{
    [SerializeField] private GameObject _gameplayUIContainer;
    [SerializeField] private Button _shopToggleButton;
    [SerializeField] private TMP_Text _balanceCurrencyText;

    [Header("Shop UI")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMP_Text _refreshCostText;
    [SerializeField] private Button _refreshShopButton;
    [SerializeField] private Transform _shopUnitCardContainer;
    [SerializeField] private ShopUnitCard _shopUnitCard;

    [Header("Inventory UI")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _inventoryUnitCardContainer;
    [SerializeField] private InventoryUnitCard _inventorytUnitCard;

    private List<ShopUnitCard> _shopUnitCardList;
    private List<InventoryUnitCard> _inventoryUnitCardList;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void OnEnable()
    {
        _shopToggleButton.onClick.AddListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.AddListener(OnRefreshShopButtonClicked);
    }

    private void OnDisable()
    {
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
    }

    public void Initialize()
    {
        _shopUnitCardList = new List<ShopUnitCard>();
        _inventoryUnitCardList = new List<InventoryUnitCard>();
        _shopPanel.SetActive(false);
    }

    public void AddShopUnitCard(UnitData unitData)
    {
        ShopUnitCard newShopUnitCard = Instantiate(_shopUnitCard, _shopUnitCardContainer);
        newShopUnitCard.Initialize(unitData);
        _shopUnitCardList.Add(newShopUnitCard);
    }

    public void RemoveShopUnitCard(ShopUnitCard cardToRemove)
    {
        if (_shopUnitCardList.Contains(cardToRemove))
        {
            _shopUnitCardList.Remove(cardToRemove);
            Destroy(cardToRemove.gameObject);
        }
    }

    public void RemoveAllShopUnitCards()
    {
        foreach (var card in _shopUnitCardList)
        {
            Destroy(card.gameObject);
        }
        _shopUnitCardList.Clear();
    }

    public void AddInventoryUnitCard(UnitData unitData)
    {
        InventoryUnitCard newInventoryUnitCard = Instantiate(_inventorytUnitCard, _inventoryUnitCardContainer);
        newInventoryUnitCard.Initialize(unitData);
        _inventoryUnitCardList.Add(newInventoryUnitCard);
    }

    public void RemoveInventoryUnitCard(InventoryUnitCard cardToRemove)
    {
        if (_inventoryUnitCardList.Contains(cardToRemove))
        {
            _inventoryUnitCardList.Remove(cardToRemove);
            Destroy(cardToRemove.gameObject);
        }
    }

    private void OnShopToggleButtonClicked()
    {
        ToggleShopPanelVisibility();
    }

    private void ToggleShopPanelVisibility()
    {
        _shopPanel.SetActive(!_shopPanel.activeSelf);
    }

    public void UpdateCurrenyUI(int balance)
    {
        _balanceCurrencyText.text = balance.ToString();
    }

    public void UpdateRefreshCostUI(int cost)
    {
        _refreshCostText.text = cost.ToString();
    }

    private void OnRefreshShopButtonClicked()
    {
        ShopService shopServiceObj = GameManager.Instance.Get<ShopService>();
        shopServiceObj.RefreshShop();
    }

}
