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
    [SerializeField] private Button _enterCombatButton;
    [SerializeField] private Button _shopToggleButton;
    [SerializeField] private TMP_Text _balanceCurrencyText;
    [SerializeField] private Button _pausePlayGameplayButton;

    [Header("--Gameplay Paused UI")]
    [SerializeField] private GameObject _gameplayPausedContainer;
    [SerializeField] private Button _resumeGameplayButton;
    [SerializeField] private Button _restartRoundGameplayPausedButton;
    [SerializeField] private Button _backToStageSelectGameplayPausedButton;

    [Header("--Gameplay Start Notification UI")]
    [SerializeField] private GameObject _gameplayStartNotificationContainer;
    [SerializeField] private TMP_Text _stageStartNotificationText;
    [SerializeField] private TMP_Text _roundStartNotificationText;

    [Header("--Gameplay Over Notification UI")]
    [SerializeField] private GameObject _gameplayOverNotificationContainer;
    [SerializeField] private TMP_Text _gameplayOverStatusMessageText;
    [SerializeField] private TMP_Text _stageOverNotificationText;
    [SerializeField] private TMP_Text _roundOverNotificationText;
    [SerializeField] private GameObject _gameplayOverRewardsContainer;
    [SerializeField] private TMP_Text _rewardsQuantityText;
    [SerializeField] private Button _nextRoundGameplayOverButton;
    [SerializeField] private Button _restartRoundGameplayOverButton;
    [SerializeField] private Button _backToStageSelectGameplayOverButton;

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

    [Header("Delay Values UI")]
    [SerializeField] private float _roundStartUIDisplayDuration = 3f;

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
        ToggleGameplayPausedContainer(false);
    }

    private void OnEnable()
    {
        _chooseStageButton.onClick.AddListener(OnChooseStageButtonClicked);
        _enterCombatButton.onClick.AddListener(OnEnterCombatButtonClicked);
        _shopToggleButton.onClick.AddListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.AddListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.AddListener(OnBuyLevelXpButtonClicked);
        _startContinueStageButton.onClick.AddListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.AddListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.AddListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.AddListener(OnResetStageConfirmationNoButtonClicked);
        _pausePlayGameplayButton.onClick.AddListener(OnPausePlayGameplayToggleChanged);
        _resumeGameplayButton.onClick.AddListener(OnResumeGameplayButtonClicked);
        _nextRoundGameplayOverButton.onClick.AddListener(OnNextRoundButtonGameplayOverClicked);
        _restartRoundGameplayOverButton.onClick.AddListener(OnRestartRoundGameplayOverClicked);
        //_backToStageSelectGameplayOverButton.onClick.AddListener();
    }

    private void OnDisable()
    {
        _chooseStageButton.onClick.RemoveListener(OnChooseStageButtonClicked);
        _enterCombatButton.onClick.RemoveListener(OnEnterCombatButtonClicked);
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.RemoveListener(OnBuyLevelXpButtonClicked);
        _startContinueStageButton.onClick.RemoveListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.RemoveListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.RemoveListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.RemoveListener(OnResetStageConfirmationNoButtonClicked);
        _pausePlayGameplayButton.onClick.RemoveListener(OnPausePlayGameplayToggleChanged);
        _resumeGameplayButton.onClick.RemoveListener(OnResumeGameplayButtonClicked);
        _nextRoundGameplayOverButton.onClick.RemoveListener(OnNextRoundButtonGameplayOverClicked);
        _restartRoundGameplayOverButton.onClick.RemoveListener(OnRestartRoundGameplayOverClicked);
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
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayPaused, OnGameplayPaused_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayResumed, OnGameplayResumed_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundStarted, OnRoundStarted);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOver, OnRoundOver);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageOver, OnStageOver);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageFailed, OnStageFailed);
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
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayPaused, OnGameplayPaused_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayResumed, OnGameplayResumed_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundStarted, OnRoundStarted);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOver, OnRoundOver);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageOver, OnStageOver);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageFailed, OnStageFailed);
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

    public void RemoveAllInventoryUnitCard()
    {
        foreach (var card in _inventoryUnitCardList)
        {
            Destroy(card.gameObject);
        }
        _inventoryUnitCardList.Clear();
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

    private void OnEnterCombatButtonClicked()
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

    private void OnPausePlayGameplayToggleChanged()
    {
        GameplayManager.Instance.PauseGameplay();
    }

    private void OnGameplayResumed_UI(object[] parameters)
    {
        ToggleGameplayPausedContainer(false);
    }

    private void OnGameplayPaused_UI(object[] parameters)
    {
        ToggleGameplayPausedContainer(true);
    }

    private void ToggleGameplayPausedContainer(bool value)
    {
        _gameplayPausedContainer.SetActive(value);
    }

    private void OnResumeGameplayButtonClicked()
    {
        GameplayManager.Instance.ResumeGameplay();
    }

    private void OnRoundStarted(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];

        SetGameplayStartStageText(stageIndex + 1);
        SetGameplayStartRoundText(roundIndex + 1);

        StartCoroutine(HandleRoundStartNotificationRoutine());
    }

    private IEnumerator HandleRoundStartNotificationRoutine()
    {
        ToggleGameplayStartNoticationContainer(true);
        yield return new WaitForSeconds(_roundStartUIDisplayDuration);
        ToggleGameplayStartNoticationContainer(false);
    }


    private void OnRoundOver(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];

        SetGameplayOverStageText(stageIndex + 1);
        SetGameplayOverRoundText(roundIndex + 1);

        RoundResultEnum result = (RoundResultEnum)parameters[2];
        int rewardQuantity = (int)parameters[3];

        string statusMessageText = string.Empty;

        switch (result)
        {
            case RoundResultEnum.Win:
                statusMessageText = "Round Won!";
                break;
            case RoundResultEnum.Lose:
                statusMessageText = "Round Lost!";
                break;
            case RoundResultEnum.Draw:
                statusMessageText = "Round Draw!";
                break;
        }

        SetRewardsQuantityText(rewardQuantity);
        SetGameplayOverStatusMessageText(statusMessageText);
        ToggleGameplayOverNoticationContainer(true);
    }

    private void OnStageOver(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        Debug.Log($"Stage {stageIndex + 1} Completed!");
    }

    private void OnStageFailed(object[] parameters)
    {
        Debug.Log("Stage Failed!");
    }

    private void ToggleGameplayStartNoticationContainer(bool value)
    {
        _gameplayStartNotificationContainer.SetActive(value);
    }

    private void ToggleGameplayOverNoticationContainer(bool value)
    {
        _gameplayOverNotificationContainer.SetActive(value);
    }

    private void ToggleGameplayOverRewardsContainer(bool value)
    {
        _gameplayOverRewardsContainer.SetActive(value);
    }

    private void ToggleGameplayOverNextRoundButton(bool value)
    {
        _nextRoundGameplayOverButton.gameObject.SetActive(value);
    }

    private void SetGameplayStartStageText(int value)
    {
        _stageStartNotificationText.text = "Stage " + value.ToString();
    }

    private void SetGameplayStartRoundText(int value)
    {
        _roundStartNotificationText.text = "Round " + value.ToString();
    }

    private void SetGameplayOverStageText(int value)
    {
        _stageOverNotificationText.text = "Stage " + value.ToString();
    }

    private void SetGameplayOverRoundText(int value)
    {
        _roundOverNotificationText.text = "Round " + value.ToString();
    }

    private void SetGameplayOverStatusMessageText(string value)
    {
        _gameplayOverStatusMessageText.text = value.ToString();
    }

    private void SetRewardsQuantityText(int value)
    {
        _rewardsQuantityText.text = value.ToString();
    }

    public void OnNextRoundButtonGameplayOverClicked()
    {
        ToggleGameplayOverNoticationContainer(false);
        GameplayManager.Instance.OnPlayerChooseNextRound();
    }

    public void OnRestartRoundGameplayOverClicked()
    {
        ToggleGameplayOverNoticationContainer(false);
        GameplayManager.Instance.OnPlayerChooseRestartRound();
    }
}
