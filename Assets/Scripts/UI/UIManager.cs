using AutoBattler.Event;
using AutoBattler.Main;
using AutoBattler.Utilities;
using System.Collections;
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

    [Header("Buff UI")]
    [SerializeField] private GameObject _buffTeam1ToggleContent;
    [SerializeField] private GameObject _buffTeam2ToggleContent;
    [SerializeField] private Transform _buffTeam1Container;
    [SerializeField] private Transform _buffTeam2Container;
    [SerializeField] private Toggle _team1ToggleButton;
    [SerializeField] private Toggle _team2ToggleButton;
    [SerializeField] private BuffDetailsUICard _buffBlockUICardPrefab;

    [Header("Level XP UI")]
    [SerializeField] private TMP_Text _currentLevelText;
    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _xpPurchaseCostText;
    [SerializeField] private Image _levelXpBarBackgroundImage;
    [SerializeField] private Image _levelXpBarfillImage;
    [SerializeField] private Button _buyLevelXpButton;
    [SerializeField] private float _maxFillAmount = 0.75f;
    [SerializeField] private float _roatationForLevelXPBar =45f;
    [SerializeField] private float _xpLerpSpeed = 8f;
    private float _displayedXP;
    private float _targetXP;
    private Coroutine _xpRoutine;


    private List<ShopUnitCard> _shopUnitCardList;
    private List<InventoryUnitCard> _inventoryUnitCardList;
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam1UICardDictionary = new();
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam2UICardDictionary = new();

    public Canvas UICanvas => _uiCanvas;

    public RectTransform CanvasRect { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CanvasRect = _uiCanvas.GetComponent<RectTransform>();
        SetupLevelXPBarUI();
        ToggleDiscardPanelVisibility(false);
        HandleTeamBuffTabSwitch(true, 1);
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
        _team1ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.AddListener(OnBuyLevelXpButtonClicked);
        EventBusManager.Instance.Subscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
    }

    void UnsubscribeToEvents()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitDragged, OnUnitDragged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.InventoryUnitCardDragged, OnInventoryUnitCardDragged_UI);
        _team1ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.RemoveListener(OnBuyLevelXpButtonClicked);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
    }

    public void InitializeGameplayUI()
    {
        _shopUnitCardList = new List<ShopUnitCard>();
        _inventoryUnitCardList = new List<InventoryUnitCard>();
        _buffTeam1UICardDictionary = new Dictionary<BuffNameEnum, BuffDetailsUICard>();
        _buffTeam2UICardDictionary = new Dictionary<BuffNameEnum, BuffDetailsUICard>();
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

    public void AddBuffDetailUICard(BuffData buffData, TeamEnum team)
    {
        BuffDetailsUICard newBuffDetailsUICard;

        switch (team)
        {
            case TeamEnum.Team1:
                newBuffDetailsUICard = Instantiate(_buffBlockUICardPrefab, _buffTeam1Container);
                newBuffDetailsUICard.Initialize(buffData);
                _buffTeam1UICardDictionary.Add(buffData.buffName, newBuffDetailsUICard);
                break;
            case TeamEnum.Team2:
                newBuffDetailsUICard = Instantiate(_buffBlockUICardPrefab, _buffTeam2Container);
                newBuffDetailsUICard.Initialize(buffData);
                _buffTeam2UICardDictionary.Add(buffData.buffName, newBuffDetailsUICard);
                break;
        }
    }

    public void RemoveAllBuffDetailUICards()
    {
        foreach (var cardData in _buffTeam1UICardDictionary)
        {
            Destroy(cardData.Value.gameObject);
        }

        foreach (var cardData in _buffTeam2UICardDictionary)
        {
            Destroy(cardData.Value.gameObject);
        }

        _buffTeam1UICardDictionary.Clear();
        _buffTeam2UICardDictionary.Clear();
    }

    public void UpdateBuffParticipantCount(BuffNameEnum buffName, int participants, TeamEnum team)
    {
        switch(team)
        {
            case TeamEnum.Team1:
                _buffTeam1UICardDictionary[buffName].ActivateParticipantBlock(participants);
                break;
            case TeamEnum.Team2:
                _buffTeam2UICardDictionary[buffName].ActivateParticipantBlock(participants);
                break;
        }
    }

    void HandleTeamBuffTabSwitch(bool isOn, int tabIndex)
    {
        if (isOn)
        {
            if (tabIndex == 1)            {
                _team2ToggleButton.isOn = false;
                _buffTeam1ToggleContent.SetActive(true);
                _buffTeam2ToggleContent.SetActive(false);
            }
            else if (tabIndex == 2)
            {
                _team1ToggleButton.isOn = false;
                _buffTeam1ToggleContent.SetActive(false);
                _buffTeam2ToggleContent.SetActive(true);
            }
        }
    }

    private void SetupLevelXPBarUI()
    {
        _levelXpBarBackgroundImage.fillAmount = _maxFillAmount;
        _levelXpBarfillImage.fillAmount = 0;
        _levelXpBarBackgroundImage.transform.rotation = Quaternion.Euler(0f, 0f, _roatationForLevelXPBar);
        _levelXpBarfillImage.transform.rotation = Quaternion.Euler(0f, 0f, _roatationForLevelXPBar);
    }

    private void OnBuyLevelXpButtonClicked()
    {
        EventBusManager.Instance.Raise(EventNameEnum.BuyLevelXP);
    }

    private void OnXPChanged_UI(object[] parameters)
    {
        _targetXP = (float)parameters[0];
        if (_xpRoutine != null)
        {
            StopCoroutine(_xpRoutine);
        }
        _xpRoutine = StartCoroutine(SmoothLevelXPFillAnimation());
    }

    private void OnLevelChanged_UI(object[] parameters)
    {
        int level = (int)parameters[0];
        _targetXP = 1f;

        if (_xpRoutine != null)
            StopCoroutine(_xpRoutine);

        _xpRoutine = StartCoroutine(SmoothLevelXPFillAnimation());

        StartCoroutine(LevelUpReset());
        _currentLevelText.text = level.ToString();
    }

    void UpdateLevelXPBar(float progressValue)
    {
        _levelXpBarfillImage.fillAmount = progressValue * _maxFillAmount;
    }

    private IEnumerator SmoothLevelXPFillAnimation()
    {
        while (true)
        {
            _displayedXP = Mathf.MoveTowards(_displayedXP, _targetXP, Time.deltaTime * _xpLerpSpeed);

            UpdateLevelXPBar(_displayedXP);

            if (Mathf.Abs(_displayedXP - _targetXP) < 0.001f)
                break;

            yield return null;
        }

        _xpRoutine = null;
    }

    private IEnumerator LevelUpReset()
    {
        while (Mathf.Abs(_displayedXP - 1f) > 0.01f)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        _displayedXP = 0f;
        _targetXP = 0f;

        UpdateLevelXPBar(0f);

        if (_xpRoutine != null)
            StopCoroutine(_xpRoutine);
        _xpRoutine = StartCoroutine(SmoothLevelXPFillAnimation());
    }
}
