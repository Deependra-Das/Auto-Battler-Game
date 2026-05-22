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

    [Header("MainMenu UI")]
    [SerializeField] private GameObject _mainMenuUIContainer;
    [SerializeField] private Button _chooseStageButton;

    [Header("StageSelection UI")]
    [SerializeField] private GameObject _stageSelectionUIContainer;
    [SerializeField] private GameObject _stageSelectionCardUIPrefab;
    [SerializeField] private Transform _stageSelectionContent;
    [SerializeField] private Button _startContinueStageButton;
    [SerializeField] private Button _resetStageButton;
    [SerializeField] private GameObject _stageSelectionConfirmationContainer;
    [SerializeField] private TMP_Text _resetStageConfirmationMessageText;
    [SerializeField] private Button _resetStageConfirmationYesButton;
    [SerializeField] private Button _resetStageConfirmationNoButton;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject _gameplayUIContainer;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopToggleButton;
    [SerializeField] private TMP_Text _balanceCurrencyText;

    [Header("--Shop UI")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMP_Text _refreshCostText;
    [SerializeField] private Button _refreshShopButton;
    [SerializeField] private Transform _shopUnitCardContainer;
    [SerializeField] private ShopUnitCard _shopUnitCard;

    [Header("--Inventory UI")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _inventoryUnitCardContainer;
    [SerializeField] private InventoryUnitCard _inventorytUnitCard;

    [Header("--Discard UI")]
    [SerializeField] private GameObject _discardUnitPanel;
    [SerializeField] private TMP_Text _refundText;

    [Header("--Buff UI")]
    [SerializeField] private GameObject _buffTeam1ToggleContent;
    [SerializeField] private GameObject _buffTeam2ToggleContent;
    [SerializeField] private Transform _buffTeam1Container;
    [SerializeField] private Transform _buffTeam2Container;
    [SerializeField] private Toggle _team1ToggleButton;
    [SerializeField] private Toggle _team2ToggleButton;
    [SerializeField] private BuffDetailsUICard _buffBlockUICardPrefab;

    [Header("--Level XP UI")]
    [SerializeField] private TMP_Text _currentLevelText;
    [SerializeField] private TMP_Text _xpText;
    [SerializeField] private TMP_Text _xpExchangeCostText;
    [SerializeField] private Image _levelXpBarBackgroundImage;
    [SerializeField] private Image _levelXpBarfillImage;
    [SerializeField] private Button _buyLevelXpButton;
    [SerializeField] private float _maxFillAmount = 0.75f;
    [SerializeField] private float _roatationForLevelXPBar = 45f;
    [SerializeField] private float _xpLerpSpeed = 8f;

    private float _displayedXP;
    private float _targetXP;
    private Coroutine _xpRoutine;
    private int _selectedStage = -1;

    private List<ShopUnitCard> _shopUnitCardList;
    private List<InventoryUnitCard> _inventoryUnitCardList;
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam1UICardDictionary = new();
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam2UICardDictionary = new();
    private List<StageSelectionCardUIView> _stageSelectionUICardList = new();    

    public Canvas UICanvas => _uiCanvas;

    public RectTransform CanvasRect { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CanvasRect = _uiCanvas.GetComponent<RectTransform>();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        CreateStageSelectionButton();
        SetupLevelXPBarUI();
        ToggleDiscardPanelVisibility(false);
        HandleTeamBuffTabSwitch(true, 1);
        ToggleMainMenuUIContainer(false);
        ToggleStageSelectionUIContainer(false);
        ToggleGameplayUIContainer(false);
        ToggleStartContinueStageButton(false);
        ToggleResetStageStageButton(false);
        ToggleStageSelectionConfirmationContainer(false);
    }

    private void OnEnable()
    {
        _chooseStageButton.onClick.AddListener(OnChooseStageButtonClicked);
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _shopToggleButton.onClick.AddListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.AddListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.AddListener(OnBuyLevelXpButtonClicked);
        _startContinueStageButton.onClick.AddListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.AddListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.AddListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.AddListener(OnResetStageConfirmationNoButtonClicked);
    }

    private void OnDisable()
    {
        _chooseStageButton.onClick.RemoveListener(OnChooseStageButtonClicked);
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.RemoveListener(OnBuyLevelXpButtonClicked);
        _startContinueStageButton.onClick.RemoveListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.RemoveListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.RemoveListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.RemoveListener(OnResetStageConfirmationNoButtonClicked);
    }

    public void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.UnitDragged, OnUnitDragged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.InventoryUnitCardDragged, OnInventoryUnitCardDragged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.SceneLoaded, OnSceneLoaded_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStarted_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.SelectedStageChanged, OnSelectedStageChanged_UI);
    }

    private void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitDragged, OnUnitDragged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.InventoryUnitCardDragged, OnInventoryUnitCardDragged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.SceneLoaded, OnSceneLoaded_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStarted_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.SelectedStageChanged, OnSelectedStageChanged_UI);
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

    public void UpdateXpExchangeCostUI(int cost)
    {
        _xpExchangeCostText.text = cost.ToString();
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
        switch (team)
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
            if (tabIndex == 1)
            {
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
        int currentXP = (int)parameters[1];
        int requiredXPToNextLevel = (int)parameters[2];

        if (_xpRoutine != null)
        {
            StopCoroutine(_xpRoutine);
        }

        _xpRoutine = StartCoroutine(SmoothLevelXPFillAnimation());
        UpdateXPText(currentXP, requiredXPToNextLevel);
    }

    private void OnLevelChanged_UI(object[] parameters)
    {
        int level = (int)parameters[0];
        int maxUnitsAllowedOnField = (int)parameters[1];
        int currentXP = (int)parameters[2];
        int requiredXPToNextLevel = (int)parameters[3];

        if (level > 1)
        {     
            _targetXP = 1f;

            if (_xpRoutine != null)
            {
                StopCoroutine(_xpRoutine);
            }

            _xpRoutine = StartCoroutine(SmoothLevelXPFillAnimation());

            StartCoroutine(LevelUpReset());
        }

        UpdateLevelText(level);
        UpdateXPText(currentXP, requiredXPToNextLevel);
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

    private void UpdateLevelText(int currentPlayerLevel)
    {
        _currentLevelText.text = currentPlayerLevel.ToString();
    }

    private void UpdateXPText(int currentXP, int requiredXPToNextLevel)
    {
        _xpText.text = currentXP.ToString() + "/" + requiredXPToNextLevel.ToString();
    }

    private void CreateStageSelectionButton()
    {
        StageService stageObj = GameManager.Instance.Get<StageService>();
        int totalStages = stageObj.GetStageCount();

        for (int index = 0; index < totalStages; index++)
        {
            StageData stageData = stageObj.GetStageData(index);
            GameObject stageButton = Instantiate(_stageSelectionCardUIPrefab, _stageSelectionContent);
            StageSelectionCardUIView stageSelectionCardUIViewObj = stageButton.GetComponent<StageSelectionCardUIView>();
            stageSelectionCardUIViewObj.Initialize(index, stageData.stageName, stageData.roundDataList.Count);
            _stageSelectionUICardList.Add(stageSelectionCardUIViewObj);
        }
    }

    private void OnChooseStageButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneNameEnum.StageSelectionScene);
    }

    private void OnStartContinueStageButtonClicked()
    {
        GameData.selectedStage = _selectedStage;
        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene);
    }

    private void OnResetStageButtonClicked()
    {
        SetMessageForResetStageConfirmation();
        ToggleStageSelectionConfirmationContainer(true);
    }

    private void OnSelectedStageChanged_UI(object[] parameters)
    {
        _selectedStage = (int)parameters[0];

        foreach (var stage in _stageSelectionUICardList)
        {
            if (stage.StageIndex == _selectedStage)
                stage.SetStageCardUISelectedHighlight(true);
            else
                stage.SetStageCardUISelectedHighlight(false);
        }

        ToggleStartContinueStageButton(true);
        ToggleResetStageStageButton(true);
    }

    public void ToggleMainMenuUIContainer(bool value)
    {
        _mainMenuUIContainer.SetActive(value);
    }

    public void ToggleStageSelectionUIContainer(bool value)
    {
        _stageSelectionUIContainer.SetActive(value);
    }

    public void ToggleGameplayUIContainer(bool value)
    {
        _gameplayUIContainer.SetActive(value);
    }

    private void OnSceneLoaded_UI(object[] parameters)
    {
        SceneNameEnum sceneLoaded = (SceneNameEnum)parameters[0];

        ToggleMainMenuUIContainer(false);
        ToggleStageSelectionUIContainer(false);
        ToggleGameplayUIContainer(false);

        switch (sceneLoaded)
        {
            case SceneNameEnum.MainMenuScene:
                ToggleMainMenuUIContainer(true);
                break;

            case SceneNameEnum.StageSelectionScene:
                UpdateStageSelectionRoundData();
                ToggleStageSelectionUIContainer(true);
                break;

            case SceneNameEnum.GameplayScene:
                ToggleGameplayUIContainer(true);
                break;
        }
    }

    private void OnStageStarted_UI(object[] parameters)
    {
        UpdateXpExchangeCostUI((int)parameters[6]);
    }

    private void ToggleStartContinueStageButton(bool value)
    {
        _startContinueStageButton.interactable = value;
    }

    private void ToggleResetStageStageButton(bool value)
    {
        _resetStageButton.interactable = value;
    }

    private void ToggleStageSelectionConfirmationContainer(bool value)
    {
        _stageSelectionConfirmationContainer.SetActive(value);
    }

    private void SetMessageForResetStageConfirmation()
    {
        _resetStageConfirmationMessageText.text = "Are you sure you want to reset Stage " + (_selectedStage + 1).ToString() + " ?";
    }

    private void OnResetStageConfirmationYesButtonClicked()
    {
        GameManager.Instance.Get<StageSnapshotService>().DeleteStageSnapshot(_selectedStage);
        UpdateStageSelectionRoundData();
        ToggleStageSelectionConfirmationContainer(false);
    }

    private void OnResetStageConfirmationNoButtonClicked()
    {
        ToggleStageSelectionConfirmationContainer(false);
    }

    private void UpdateStageSelectionRoundData()
    {
        StageSnapshotService stageSnapshotService = GameManager.Instance.Get<StageSnapshotService>();

        foreach(StageSelectionCardUIView cardObj in _stageSelectionUICardList)
        {
            StageSnapshotEntry data = stageSnapshotService.GetStageSnapshot(cardObj.StageIndex);

            if (data != null)
            {
                cardObj.SetStageRoundData(data.winCount+data.drawCount);
            }
            else
            {
                cardObj.SetStageRoundData(0);
            }
        }
    }
}
