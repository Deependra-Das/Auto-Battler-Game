using AutoBattler.Event;
using AutoBattler.Main;
using AutoBattler.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : GenericMonoSingleton<UIManager>
{
    [SerializeField] private Canvas _uiCanvas;
    [SerializeField] private Button _playButton;
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

    [Header("Discard UI")]
    [SerializeField] private GameObject _discardUnitPanel;
    [SerializeField] private TMP_Text _refundText;

    private List<ShopUnitCard> _shopUnitCardList;
    private List<InventoryUnitCard> _inventoryUnitCardList;
    public Canvas UICanvas => _uiCanvas;

    public RectTransform CanvasRect { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CanvasRect = _uiCanvas.GetComponent<RectTransform>();
        ToggleDiscardPanelVisibility(false);
        Initialize();
    }

    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked); 
        _shopToggleButton.onClick.AddListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.AddListener(OnRefreshShopButtonClicked);
        EventBusManager.Instance.Subscribe(EventNameEnum.UnitDragged, OnUnitDragged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.InventoryUnitCardDragged, OnInventoryUnitCardDragged_UI);
    }

    void UnsubscribeToEvents()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitDragged, OnUnitDragged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.InventoryUnitCardDragged, OnInventoryUnitCardDragged_UI);
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
        newInventoryUnitCard.Initialize(unitData, _uiCanvas);
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

    public void RefreshInventoryOrder()
    {
        _inventoryUnitCardList.Clear();

        List<UnitData> newOrder = new();

        for (int i = 0; i < _inventoryUnitCardContainer.childCount; i++)
        {
            InventoryUnitCard card = _inventoryUnitCardContainer.GetChild(i).GetComponent<InventoryUnitCard>();

            if (card == null)
                continue;

            _inventoryUnitCardList.Add(card);
            newOrder.Add(card.UnitData);
        }

        InventoryService inventory = GameManager.Instance.Get<InventoryService>();

        inventory.ReorderUnits(newOrder);
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

    private void OnPlayButtonClicked()
    {
        GameplayManager.Instance.UpdateGameplayState(GameplayStateEnum.Combat);
    }

    private void OnUnitDragged_UI(object[] parameters)
    {
        bool value = (bool)parameters[0];
        ToggleDiscardPanelVisibility(value);
    }

    private void OnInventoryUnitCardDragged_UI(object[] parameters)
    {
        bool value = (bool)parameters[0];
        ToggleDiscardPanelVisibility(value);
    }

    private void ToggleDiscardPanelVisibility(bool value)
    {
        _discardUnitPanel.SetActive(value);
    }
}
