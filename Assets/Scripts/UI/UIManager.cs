using AutoBattler.Event;
using AutoBattler.Main;
using AutoBattler.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : GenericMonoSingleton<UIManager>
{
    [SerializeField] private Canvas _uiCanvas;

    [Header("MainMenu UI")]
    [SerializeField] private GameObject _mainMenuUIContainer;
    [SerializeField] private Button _mainMenuPlayButton;
    [SerializeField] private Button _audioSettingsButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _exitGameButton;
    [SerializeField] private Graphic _flashingGraphic; 
    [SerializeField] private float _fadeDuration = 1f;

    [Header("HowToPlay UI")]
    [SerializeField] private GameObject _howToPlayUIContainer;
    [SerializeField] private Button _closeHowToPlayButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _currentInstructionIndexText;
    [SerializeField] private TMP_Text _totalInstructionCountText;
    [SerializeField] private TMP_Text _instructionTitleText;
    [SerializeField] private TMP_Text _instructionDescriptionText;

    [Header("Credits UI")]
    [SerializeField] private GameObject _creditsUIContainer;
    [SerializeField] private Button _closeCreditsButton;

    [Header("AudioSettings UI")]
    [SerializeField] private GameObject _audioSettingsUIContainer;
    [SerializeField] private Button _closeAudioSettingsButton;
    [SerializeField] private Sprite _mutedIconSprite;
    [SerializeField] private Sprite _unmutedIconSprite;
    [SerializeField] private AudioControlUI[] _audioControls;

    [Header("StageSelection UI")]
    [SerializeField] private GameObject _stageSelectionUIContainer;
    [SerializeField] private GameObject _stageSelectionCardUIPrefab;
    [SerializeField] private Transform _stageSelectionContent;
    [SerializeField] private Button _startContinueStageButton;
    [SerializeField] private Button _resetStageButton;
    [SerializeField] private Button _closeStageSelectionButton;
    [SerializeField] private GameObject _stageSelectionConfirmationContainer;
    [SerializeField] private TMP_Text _resetStageConfirmationMessageText;
    [SerializeField] private Button _resetStageConfirmationYesButton;
    [SerializeField] private Button _resetStageConfirmationNoButton;
    [SerializeField] private TMP_Text _recommendedLevelText;
    [SerializeField] private List<RecommendedElementCard> _recommendedElementList;
    [SerializeField] private List<Image> _difficultyImageList;
    [SerializeField] private Image _selectedStageBackgroundImage;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject _gameplayUIContainer;
    [SerializeField] private Button _enterCombatButton;
    [SerializeField] private Button _shopToggleButton;
    [SerializeField] private TMP_Text _balanceCurrencyText;
    [SerializeField] private Button _pauseGameplayButton;
    [SerializeField] private TMP_Text _roundInfoGameplayUIText;
    [SerializeField] private TMP_Text _stageInfoGameplayUIText;
    [SerializeField] private GameObject _bottomControlPanel;
    [SerializeField] private TMP_Text _playerLivesInfoGameplayUIText;
    [SerializeField] private TMP_Text _unitsOnFieldUIText;
    [SerializeField] private TMP_Text _maxUnitsAllowedOnFieldUIText;

    [Header("--Gameplay Paused UI")]
    [SerializeField] private GameObject _gameplayPausedContainer;
    [SerializeField] private Button _resumeGameplayButton;
    [SerializeField] private Button _restartRoundPauseMenuButton;
    [SerializeField] private Button _returnToMenuGameplayPausedButton;

    [Header("--Gameplay Start Notification UI")]
    [SerializeField] private GameObject _gameplayStartNotificationContainer;
    [SerializeField] private TMP_Text _stageStartNotificationText;
    [SerializeField] private TMP_Text _roundStartNotificationText;
    [SerializeField] private Image _stageBackgroundImage;

    [Header("--Gameplay Over Notification UI")]
    [SerializeField] private GameObject _gameplayOverNotificationContainer;
    [SerializeField] private TMP_Text _stageInfoGameplayOverText;
    [SerializeField] private TMP_Text _roundInfoGameplayOverText;
    [SerializeField] private GameObject _gameplayOverRewardsContainer;
    [SerializeField] private TMP_Text _rewardsQuantityText;
    [SerializeField] private GameObject _stageOverStatusContainer;
    [SerializeField] private TMP_Text _stageOverStatusMessageText;
    [SerializeField] private TMP_Text _stageOverSubText;
    [SerializeField] private GameObject _nextRoundButtonContainer;
    [SerializeField] private Button _nextRoundGameplayOverButton;
    [SerializeField] private Button _restartRoundGameplayOverButton;
    [SerializeField] private Button _returnToMenuGameplayOverButton;

    [Header("--Shop UI")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMP_Text _refreshCostText;
    [SerializeField] private Button _refreshShopButton;
    [SerializeField] private ShopUnitCard _shopUnitCardPrefab;
    [SerializeField] private Transform _shopUnitCardActiveContainer;
    [SerializeField] private Transform _shopUnitCardPoolContainer;
    [SerializeField] private int _shopUnitCardPoolSize = 8;

    [Header("--Inventory UI")]
    [SerializeField] private InventoryUnitCard _inventoryUnitCardPrefab;
    [SerializeField] private InventoryDropZoneManager _inventoryDropZonePrefab;
    [SerializeField] private Transform _inventoryUnitCardActiveContainer;
    [SerializeField] private Transform _inventoryUnitCardPoolContainer;
    [SerializeField] private int _inventoryUnitCardPoolSize = 8;
    [SerializeField] private RectTransform _dragVisualPoolContainerRectTransform;

    [Header("--Discard UI")]
    [SerializeField] private DiscardUnitDropZoneManager _discardUnitDropZonePrefab;
    [SerializeField] private Transform _unitPoolContainer;

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
    [SerializeField] private float _maxFillAmount = 0.92f;
    [SerializeField] private float _roatationForLevelXPBar = -104f;
    [SerializeField] private float _xpLerpSpeed = 8f;

    [Header("Delay Values UI")]
    [SerializeField] private float _roundStartUIDisplayDuration = 3f;

    private float _displayedXP;
    private float _targetXP;
    private Coroutine _xpRoutine;
    private Coroutine _flashCoroutine;
    private int _selectedStage = -1;
    private int _currentInstructionIndex;
    private int _totalInstructionCount = 0;
    private bool _initialBuffTabSwitched = false;

    private List<ShopUnitCard> _shopUnitCardList;
    private List<InventoryUnitCard> _inventoryUnitCardList;
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam1UICardDictionary = new();
    private Dictionary<BuffNameEnum, BuffDetailsUICard> _buffTeam2UICardDictionary = new();
    private List<StageSelectionCardUIView> _stageSelectionUICardList = new();

    private ShopUnitCardPool _shopCardPoolObj;
    private InventoryUnitCardPool _inventoryCardPoolObj;
    private DiscardUnitDropZoneManager _discardUnitDropZoneManagerObj;
    private InventoryDropZoneManager _inventoryDropZoneManagerObj;
    private VideoInstructionService _videoInstructionServiceObj;
    private ShopService _shopServiceObj;

    public Canvas UICanvas => _uiCanvas;

    public RectTransform CanvasRect { get; private set; }

    public RectTransform DragVisualPoolContainerRectTransform => _dragVisualPoolContainerRectTransform;

    protected override void Awake()
    {
        base.Awake();
        CanvasRect = _uiCanvas.GetComponent<RectTransform>();
        _shopCardPoolObj = new ShopUnitCardPool( _shopUnitCardPrefab, _shopUnitCardActiveContainer, _shopUnitCardPoolContainer, _shopUnitCardPoolSize);
        _inventoryCardPoolObj = new InventoryUnitCardPool(_inventoryUnitCardPrefab, _inventoryUnitCardActiveContainer, _inventoryUnitCardPoolContainer, _inventoryUnitCardPoolSize);
    }

    public void Initialize()
    {
        SubscribeToEvents();
        SetupLevelXPBarUI();
        HandleTeamBuffTabSwitch(true, 1);
        ToggleMainMenuUIContainer(false);
        ToggleStageSelectionUIContainer(false);
        ToggleCreditsUI(false);
        ToggleAudioSettingsUI(false);
        ToggleHowToPlayUI(false);
        ToggleGameplayUIContainer(false);
        ToggleStartContinueStageButton(false);
        ToggleResetStageStageButton(false);
        ToggleGameplayStartNoticationContainer(false);
        ToggleStageSelectionConfirmationContainer(false);
        ToggleGameplayPausedContainer(false);
        InitializeAudioUI();
        _videoInstructionServiceObj = GameManager.Instance.Get<VideoInstructionService>();
        _shopServiceObj = GameManager.Instance.Get<ShopService>();
    }

    private void OnEnable()
    {
        _mainMenuPlayButton.onClick.AddListener(OnMainMenuPlayButtonClicked);
        _exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
        _audioSettingsButton.onClick.AddListener(OnAudioSettingsButtonClicked);
        _howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
        _creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        _closeAudioSettingsButton.onClick.AddListener(OnCloseAudioSettingsButtonClicked);
        _closeHowToPlayButton.onClick.AddListener(OnCloseHowToPlayButtonClicked);
        _closeCreditsButton.onClick.AddListener(OnCloseCreditsButtonClicked);
        _previousButton.onClick.AddListener(OnPreviousButtonClicked);
        _nextButton.onClick.AddListener(OnNextButtonClicked);

        _startContinueStageButton.onClick.AddListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.AddListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.AddListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.AddListener(OnResetStageConfirmationNoButtonClicked);
        _closeStageSelectionButton.onClick.AddListener(OnCloseStageSelectionButtonClicked);

        _enterCombatButton.onClick.AddListener(OnEnterCombatButtonClicked);
        _shopToggleButton.onClick.AddListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.AddListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.AddListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.AddListener(OnBuyLevelXpButtonClicked);
        
        _pauseGameplayButton.onClick.AddListener(OnPausePlayGameplayToggleChanged);
        _resumeGameplayButton.onClick.AddListener(OnResumeGameplayButtonClicked);
        _restartRoundPauseMenuButton.onClick.AddListener(OnRestartRoundPauseMenuButtonClicked);
        _nextRoundGameplayOverButton.onClick.AddListener(OnNextRoundButtonGameplayOverButtonClicked);
        _restartRoundGameplayOverButton.onClick.AddListener(OnRestartRoundGameplayOverButtonClicked);
        _returnToMenuGameplayOverButton.onClick.AddListener(OnBackToStageSelectionGameplayOverButtonClicked);
        _returnToMenuGameplayPausedButton.onClick.AddListener(OnBackToStagePauseMenuButtonClicked);
    }

    private void OnDisable()
    {
        _mainMenuPlayButton.onClick.RemoveListener(OnMainMenuPlayButtonClicked);
        _exitGameButton.onClick.RemoveListener(OnExitGameButtonClicked);
        _audioSettingsButton.onClick.RemoveListener(OnAudioSettingsButtonClicked);
        _howToPlayButton.onClick.RemoveListener(OnHowToPlayButtonClicked);
        _creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
        _closeAudioSettingsButton.onClick.RemoveListener(OnCloseAudioSettingsButtonClicked);
        _closeHowToPlayButton.onClick.RemoveListener(OnCloseHowToPlayButtonClicked);
        _closeCreditsButton.onClick.RemoveListener(OnCloseCreditsButtonClicked);
        _previousButton.onClick.RemoveListener(OnPreviousButtonClicked);
        _nextButton.onClick.RemoveListener(OnNextButtonClicked);

        _startContinueStageButton.onClick.RemoveListener(OnStartContinueStageButtonClicked);
        _resetStageButton.onClick.RemoveListener(OnResetStageButtonClicked);
        _resetStageConfirmationYesButton.onClick.RemoveListener(OnResetStageConfirmationYesButtonClicked);
        _resetStageConfirmationNoButton.onClick.RemoveListener(OnResetStageConfirmationNoButtonClicked);
        _closeStageSelectionButton.onClick.RemoveListener(OnCloseStageSelectionButtonClicked);

        _enterCombatButton.onClick.RemoveListener(OnEnterCombatButtonClicked);
        _shopToggleButton.onClick.RemoveListener(OnShopToggleButtonClicked);
        _refreshShopButton.onClick.RemoveListener(OnRefreshShopButtonClicked);
        _team1ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 1));
        _team2ToggleButton.onValueChanged.RemoveListener((isOn) => HandleTeamBuffTabSwitch(isOn, 2));
        _buyLevelXpButton.onClick.RemoveListener(OnBuyLevelXpButtonClicked);
       
        _pauseGameplayButton.onClick.RemoveListener(OnPausePlayGameplayToggleChanged);
        _resumeGameplayButton.onClick.RemoveListener(OnResumeGameplayButtonClicked);
        _restartRoundPauseMenuButton.onClick.RemoveListener(OnRestartRoundPauseMenuButtonClicked);
        _nextRoundGameplayOverButton.onClick.RemoveListener(OnNextRoundButtonGameplayOverButtonClicked);
        _restartRoundGameplayOverButton.onClick.RemoveListener(OnRestartRoundGameplayOverButtonClicked);
        _returnToMenuGameplayOverButton.onClick.RemoveListener(OnBackToStageSelectionGameplayOverButtonClicked);
        _returnToMenuGameplayPausedButton.onClick.RemoveListener(OnBackToStagePauseMenuButtonClicked);
    }

    public void OnDestroy()
    {
        if (!IsSingletonInstance())
            return;
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.ToggleInventoryDropZone, OnToggleInventoryDropZone_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.ToggleDiscardDropZone, OnToggleDiscardDropZone_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.InventoryUnitCardDiscarded, OnInventoryUnitCardDiscarded_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.ReorderInventoryLayout, OnReorderInventoryLayout_UI);        
        EventBusManager.Instance.Subscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.SceneLoaded, OnSceneLoaded_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageStarted, OnStageStarted_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.SelectedStageChanged, OnSelectedStageChanged_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayPaused, OnGameplayPaused_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayResumed, OnGameplayResumed_UI);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundStarted, OnRoundStarted);
        EventBusManager.Instance.Subscribe(EventNameEnum.RoundOver, OnRoundOver);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageClearedFull, OnStageClearedFull);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageClearedPartial, OnStageClearedPartial);
        EventBusManager.Instance.Subscribe(EventNameEnum.StageFailed, OnStageFailed);
        EventBusManager.Instance.Subscribe(EventNameEnum.GameplayStateChanged, OnGameplayStateChanged);
        EventBusManager.Instance.Subscribe(EventNameEnum.FieldUnitsUpdated, OnUnitUpdatedOnField_UI);
    }

    private void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.ToggleInventoryDropZone, OnToggleInventoryDropZone_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.ToggleDiscardDropZone, OnToggleDiscardDropZone_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.InventoryUnitCardDiscarded, OnInventoryUnitCardDiscarded_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.ReorderInventoryLayout, OnReorderInventoryLayout_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.XPChanged, OnXPChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.LevelChanged, OnLevelChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.SceneLoaded, OnSceneLoaded_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageStarted, OnStageStarted_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.SelectedStageChanged, OnSelectedStageChanged_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayPaused, OnGameplayPaused_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayResumed, OnGameplayResumed_UI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundStarted, OnRoundStarted);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.RoundOver, OnRoundOver);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageClearedFull, OnStageClearedFull);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageClearedPartial, OnStageClearedPartial);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.StageFailed, OnStageFailed);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.GameplayStateChanged, OnGameplayStateChanged);
    }

    public void InitializeGameplayUI()
    {
        _shopUnitCardList = new List<ShopUnitCard>();
        _inventoryUnitCardList = new List<InventoryUnitCard>();
        _buffTeam1UICardDictionary = new Dictionary<BuffNameEnum, BuffDetailsUICard>();
        _buffTeam2UICardDictionary = new Dictionary<BuffNameEnum, BuffDetailsUICard>();
        _shopPanel.SetActive(false);
        CreateDiscardUnitDropZone();
        ToggleDiscardDropZoneVisibility(false);
        CreateInventoryDropZone();
        ToggleInventoryDropZoneVisibility(false);
    }

    public void AddShopUnitCard(UnitData unitData)
    {
        ShopUnitCard shopUnitCard = _shopCardPoolObj.Get();
        shopUnitCard.Initialize(unitData);
        _shopUnitCardList.Add(shopUnitCard);
    }

    public void RemoveShopUnitCard(ShopUnitCard cardToRemove)
    {
        if (_shopUnitCardList.Remove(cardToRemove))
        {
            _shopCardPoolObj.Release(cardToRemove);
        }
    }

    public void RemoveAllShopUnitCards()
    {
        for (int index = _shopUnitCardList.Count - 1; index >= 0; index--)
        {
            RemoveShopUnitCard(_shopUnitCardList[index]);
        }
        _shopUnitCardList.Clear();
    }

    public void AddInventoryUnitCard(UnitData unitData)
    {
        InventoryUnitCard newInventoryUnitCard = _inventoryCardPoolObj.Get();
        newInventoryUnitCard.Initialize(unitData, _uiCanvas, _inventoryUnitCardActiveContainer as RectTransform);
        _inventoryUnitCardList.Add(newInventoryUnitCard);
    }

    public void RemoveInventoryUnitCard(InventoryUnitCard cardToRemove)
    {
        if (_inventoryUnitCardList.Remove(cardToRemove))
        {
            _inventoryCardPoolObj.Release(cardToRemove);
        }
    }

    public void RemoveAllInventoryUnitCard()
    {   
        for (int index = _inventoryUnitCardList.Count - 1; index >= 0; index--)
        {
            RemoveInventoryUnitCard(_inventoryUnitCardList[index]);
        }

        _inventoryUnitCardList.Clear();
    }

    public void RefreshInventoryOrder()
    {
        _inventoryUnitCardList.Clear();

        List<UnitData> newOrder = new();

        for (int i = 0; i < _inventoryUnitCardActiveContainer.childCount; i++)
        {
            InventoryUnitCard card = _inventoryUnitCardActiveContainer.GetChild(i).GetComponent<InventoryUnitCard>();

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
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
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
        _shopServiceObj.RefreshShop();
    }

    private void OnEnterCombatButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        GameplayManager.Instance.UpdateGameplayState(GameplayStateEnum.Combat);
    }

    private void OnToggleInventoryDropZone_UI(object[] parameters)
    {
        bool value = (bool)parameters[0];
        ToggleInventoryDropZoneVisibility(value);
    }

    private void OnToggleDiscardDropZone_UI(object[] parameters)
    {
        bool value = (bool)parameters[0];
        ToggleDiscardDropZoneVisibility(value);
    }

    private void OnReorderInventoryLayout_UI(object[] parameters)
    {
        ReorderInventoryRebuildLayout();
    }

    private void OnInventoryUnitCardDiscarded_UI(object[] parameters)
    {
        StartCoroutine(RefreshInventoryLayoutNextFrame());
    }

    public void ReorderInventoryRebuildLayout()
    {
        RefreshInventoryOrder();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_inventoryUnitCardActiveContainer as RectTransform);
    }

    private IEnumerator RefreshInventoryLayoutNextFrame()
    {
        yield return null;
        ReorderInventoryRebuildLayout();
    }

    private void CreateDiscardUnitDropZone()
    {
        _discardUnitDropZoneManagerObj = Instantiate(_discardUnitDropZonePrefab, _bottomControlPanel.transform);
    }

    public void DestroyDiscardUnitDropZone()
    {
        if (_discardUnitDropZoneManagerObj != null)
        {
            Destroy(_discardUnitDropZoneManagerObj.gameObject);
            _discardUnitDropZoneManagerObj = null;
        }
    }

    private void ToggleDiscardDropZoneVisibility(bool value)
    {
        _discardUnitDropZoneManagerObj.gameObject.SetActive(value);
    }

    private void CreateInventoryDropZone()
    {
        _inventoryDropZoneManagerObj = Instantiate(_inventoryDropZonePrefab, _bottomControlPanel.transform);
    }

    public void DestroyInventoryDropZone()
    {
        if (_inventoryDropZoneManagerObj != null)
        {
            Destroy(_inventoryDropZonePrefab.gameObject);
            _inventoryDropZoneManagerObj = null;
        }
    }

    private void ToggleInventoryDropZoneVisibility(bool value)
    {
        _inventoryDropZoneManagerObj.gameObject.SetActive(value);
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

    public void UpdateBuffParticipantCountUI(BuffNameEnum buffName, int participants, TeamEnum team)
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

    public void ResetBuffParticipantCountUI(TeamEnum team)
    {
        switch (team)
        {
            case TeamEnum.Team1:
                _buffTeam1UICardDictionary.Values.ToList().ForEach(card => card.DeactivateParticipantBlock());
                break;
            case TeamEnum.Team2:
                _buffTeam2UICardDictionary.Values.ToList().ForEach(card => card.DeactivateParticipantBlock());
                break;
        }
    }

    private void OnUnitUpdatedOnField_UI(object[] parameters)
    {
        TeamEnum team = (TeamEnum)parameters[0];
        int fieldUnitCount = (int)parameters[1];

        if(team == TeamEnum.Team1)
        {
            UpdateUnitsOnFieldUIText(fieldUnitCount);
        }
    }

    void HandleTeamBuffTabSwitch(bool isOn, int tabIndex)
    {
        if (isOn)
        {

            if (_initialBuffTabSwitched)
            {
                AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.CardClick);
            }
            _initialBuffTabSwitched = true;

            if (tabIndex == 1)
            {
                _team2ToggleButton.isOn = false;
                _buffTeam1ToggleContent.SetActive(true);
                _buffTeam2ToggleContent.SetActive(false);
                _team1ToggleButton.image.color = Color.white;
                _team2ToggleButton.image.color = Color.grey;
            }
            else if (tabIndex == 2)
            {
                _team1ToggleButton.isOn = false;
                _buffTeam1ToggleContent.SetActive(false);
                _buffTeam2ToggleContent.SetActive(true);
                _team2ToggleButton.image.color = Color.white;
                _team1ToggleButton.image.color = Color.grey;
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
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.BuyXP);
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
        UpdateMaxUnitsAllowedOnFieldUIText(maxUnitsAllowedOnField);
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
    
    private void UpdateMaxUnitsAllowedOnFieldUIText(int maxUnitsAllowedOnField)
    {
        _maxUnitsAllowedOnFieldUIText.text = maxUnitsAllowedOnField.ToString("D2");
    }

    private void UpdateUnitsOnFieldUIText(int unitsOnField)
    {
        _unitsOnFieldUIText.text = unitsOnField.ToString("D2");
    }

    private void UpdateXPText(int currentXP, int requiredXPToNextLevel)
    {
        _xpText.text = currentXP.ToString() + " / " + requiredXPToNextLevel.ToString();
    }

    private void CreateStageSelectionButtons()
    {
        StageService stageObj = GameManager.Instance.Get<StageService>();
        int totalStages = stageObj.GetStageCount();

        for (int index = 0; index < totalStages; index++)
        {
            StageData stageData = stageObj.GetStageData(index);
            GameObject stageButton = Instantiate(_stageSelectionCardUIPrefab, _stageSelectionContent);
            StageSelectionCardUIView stageSelectionCardUIViewObj = stageButton.GetComponent<StageSelectionCardUIView>();
            stageSelectionCardUIViewObj.Initialize(index, stageData);
            _stageSelectionUICardList.Add(stageSelectionCardUIViewObj);
        }

        StartCoroutine(SelectFirstStage());
    }

    private IEnumerator SelectFirstStage()
    {
        yield return null;

        if (_stageSelectionUICardList.Count > 0)
        {
            EventBusManager.Instance.Raise(EventNameEnum.SelectedStageChanged, 0);
        }
    }

    private void ClearStageSelectionButtons()
    {
        if (_stageSelectionUICardList == null || _stageSelectionUICardList.Count == 0)
        {
            return;
        }

        foreach (StageSelectionCardUIView card in _stageSelectionUICardList)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        _stageSelectionUICardList.Clear();
    }

    private void OnMainMenuPlayButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        StopFlashing();
        SceneLoader.Instance.LoadScene(SceneNameEnum.StageSelectionScene);
    }

    private void OnStartContinueStageButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        GameData.selectedStage = _selectedStage;
        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene);
    }

    private void OnResetStageButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.Popup);
        SetMessageForResetStageConfirmation();
        ToggleStageSelectionConfirmationContainer(true);
    }

    private void OnCloseStageSelectionButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.Popup);
        SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene);
    }    

    private void OnSelectedStageChanged_UI(object[] parameters)
    {
        _selectedStage = (int)parameters[0];
        int roundCount = 0;
        _selectedStageBackgroundImage.sprite = GameManager.Instance.Get<StageService>().GetStageBackgroundImage(_selectedStage);

        foreach (var stage in _stageSelectionUICardList)
        {
            if (stage.StageIndex == _selectedStage)
            {
                roundCount = stage.NumberOfRounds;

                SetStageRecommendedLevelOnSelectionUI(stage.RecommendedLevel);
                SetStageRecommendedElementsOnSelectionUI(stage);
                SetStageDifficultyOnSelectionUI(stage.StageDifficulty);
                stage.SetStageCardUISelectedHighlight(true);
            }
            else
            {
                stage.SetStageCardUISelectedHighlight(false);
            }
        }

        StageSnapshotEntry entry = GameManager.Instance.Get<StageSnapshotService>().GetStageSnapshot(_selectedStage);

        if (entry == null || entry.latestRoundSnapshot.roundIndex < roundCount - 1)
        {
            ToggleStartContinueStageButton(true);
        }
        else
        {
            ToggleStartContinueStageButton(false);
        }

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
        PostProcessingManager.Instance.ToggleBlur(!value);
        _gameplayUIContainer.SetActive(value);
    }

    private void OnSceneLoaded_UI(object[] parameters)
    {
        SceneNameEnum sceneLoaded = (SceneNameEnum)parameters[0];

        ToggleMainMenuUIContainer(false);
        ToggleStageSelectionUIContainer(false);
        ToggleGameplayUIContainer(false);
        ClearStageSelectionButtons();

        switch (sceneLoaded)
        {
            case SceneNameEnum.MainMenuScene:
                AudioManager.Instance.PlayMusic(AudioTypeEnum.MainMenuMusic);
                PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(true);
                ToggleMainMenuUIContainer(true);
                StartFlashing();
                break;

            case SceneNameEnum.StageSelectionScene:
                AudioManager.Instance.PlayMusic(AudioTypeEnum.MainMenuMusic);
                PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(false);
                CreateStageSelectionButtons();
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
        _resetStageConfirmationMessageText.text = "Are you sure you want to reset Stage " + (_selectedStage + 1).ToString("D2") + " ?";
    }

    private void OnResetStageConfirmationYesButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        GameManager.Instance.Get<StageSnapshotService>().DeleteStageSnapshot(_selectedStage);
        UpdateStageSelectionRoundData();
        ToggleStartContinueStageButton(true);
        ToggleStageSelectionConfirmationContainer(false);
    }

    private void OnResetStageConfirmationNoButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
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
                cardObj.SetStageRoundData(data.roundResults, (data.winCount+data.drawCount), data.latestRoundSnapshot.roundIndex);
            }
            else
            {
                cardObj.SetStageRoundData(null, 0, -1);
            }
        }    
    }

    private void OnPausePlayGameplayToggleChanged()
    {
        AudioManager.Instance.PauseMusic();
        GameplayManager.Instance.PauseGameplay();
    }

    private void OnGameplayResumed_UI(object[] parameters)
    {
        ToggleGameplayUIContainer(true);
        ToggleGameplayPausedContainer(false);
    }

    private void OnGameplayPaused_UI(object[] parameters)
    {
        ToggleGameplayUIContainer(false);
        ToggleGameplayPausedContainer(true);
    }

    private void ToggleGameplayPausedContainer(bool value)
    {
        _gameplayPausedContainer.SetActive(value);

        if(value)
        {
            AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.Popup);
        }
    }

    private void OnResumeGameplayButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        GameplayManager.Instance.ResumeGameplay();
    }

    private void OnRestartRoundPauseMenuButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        GameplayManager.Instance.OnRestartRoundFromPauseMenu();
    }

    private void OnRoundStarted(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];

        SetStageStartNotificationText(stageIndex + 1);
        SetRoundStartNotificationText(roundIndex + 1);

        SetStageInfoGameplayUIText(stageIndex + 1);
        SetRoundInfoGameplayUIText(roundIndex + 1);
        SetPlayerLivesInfoGameplayUI();
        UpdateUnitsOnFieldUIText(0);
        _stageBackgroundImage.sprite = GameManager.Instance.Get<StageService>().GetStageBackgroundImage(stageIndex);
        StartCoroutine(HandleRoundStartNotificationRoutine());
    }

    private IEnumerator HandleRoundStartNotificationRoutine()
    {
        ToggleGameplayStartNoticationContainer(true);
        yield return new WaitForSeconds(_roundStartUIDisplayDuration);
        ToggleGameplayStartNoticationContainer(false);
        ToggleGameplayUIContainer(true);
    }

    private void OnRoundOver(object[] parameters)
    {
        int stageIndex = (int)parameters[0];
        int roundIndex = (int)parameters[1];

        SetStageInfoGameplayOverText(stageIndex + 1);

        RoundResultEnum result = (RoundResultEnum)parameters[2];
        int rewardQuantity = (int)parameters[3];

        string roundInfoStatusMessage = "Round "+(roundIndex + 1).ToString("D2");

        switch (result)
        {
            case RoundResultEnum.Win:
                roundInfoStatusMessage += " Won";
                break;
            case RoundResultEnum.Lose:
                roundInfoStatusMessage += " Lost";
                break;
            case RoundResultEnum.Draw:
                roundInfoStatusMessage += " Draw";
                break;
        }

        SetRewardsQuantityText(rewardQuantity);
        SetRoundInfoGameplayOverText(roundInfoStatusMessage);
        ToggleGameplayOverRewardsContainer(true);
        ToggleGameplayOverNextRoundButton(true);
        ToggleStageOverStatusContainer(false);
        ToggleGameplayUIContainer(false);
        ToggleGameplayOverNoticationContainer(true);
        AudioManager.Instance.PauseMusic();
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.RoundOver);
    }

    private void OnStageClearedFull(object[] parameters)
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.StageCleared);
        string statusMessage = "- Stage Successfully Completed -";
        SetStageOverStatusMessageText(statusMessage);
        SetStageOverSubText("All Rounds Cleared.");
        ToggleGameplayOverRewardsContainer(false);
        ToggleGameplayOverNextRoundButton(false);
        ToggleStageOverStatusContainer(true);
    }

    private void OnStageClearedPartial(object[] parameters)
    {
        string statusMessage = "- Stage Progress Recorded -";
        SetStageOverStatusMessageText(statusMessage);
        SetStageOverSubText("Clear all Rounds to Complete the Stage.");
        ToggleGameplayOverRewardsContainer(false);
        ToggleGameplayOverNextRoundButton(false);
        ToggleStageOverStatusContainer(true);
    }

    private void OnStageFailed(object[] parameters)
    {
        string statusMessage = "- Stage Attempt Failed -";
        SetStageOverStatusMessageText(statusMessage);
        SetStageOverSubText("You ran out of lives before clearing all rounds. Please Try again.");
        ToggleGameplayOverRewardsContainer(false);
        ToggleGameplayOverNextRoundButton(false);
        ToggleStageOverStatusContainer(true);
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

    private void ToggleStageOverStatusContainer(bool value)
    {
        _stageOverStatusContainer.SetActive(value);
    }

    private void ToggleGameplayOverNextRoundButton(bool value)
    {
        _nextRoundGameplayOverButton.gameObject.SetActive(value);
        _nextRoundButtonContainer.SetActive(value);
    }

    private void SetStageStartNotificationText(int value)
    {
        _stageStartNotificationText.text = "Stage " + value.ToString("D2");
    }

    private void SetRoundStartNotificationText(int value)
    {
        _roundStartNotificationText.text = "Round " + value.ToString("D2");
    }

    private void SetStageInfoGameplayOverText(int value)
    {
        _stageInfoGameplayOverText.text = "Stage " + value.ToString("D2");
    }

    private void SetRoundInfoGameplayOverText(string value)
    {
        _roundInfoGameplayOverText.text = value.ToString();
    }

    private void SetStageOverStatusMessageText(string value)
    {
        _stageOverStatusMessageText.text = value.ToString();
    }

    private void SetStageOverSubText(string value)
    {
        _stageOverSubText.text = value.ToString();
    }

    private void SetRewardsQuantityText(int value)
    {
        _rewardsQuantityText.text = value.ToString();
    }

    private void OnNextRoundButtonGameplayOverButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        ToggleGameplayOverNoticationContainer(false);
        GameplayManager.Instance.OnPlayerChooseNextRound();
    }

    private void OnRestartRoundGameplayOverButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        ToggleGameplayOverNoticationContainer(false);
        GameplayManager.Instance.OnPlayerChooseRestartRound();
    }

    private void OnBackToStageSelectionGameplayOverButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        ToggleGameplayOverNoticationContainer(false);
        GameplayManager.Instance.OnPlayerLeaveStageGameplayOver();
    }

    private void OnBackToStagePauseMenuButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        AudioManager.Instance.ResumeMusic();
        ToggleGameplayPausedContainer(false);
        GameplayManager.Instance.OnPlayerLeaveStageFromPauseMenu();
    }

    private void SetStageInfoGameplayUIText(int value)
    {
        _stageInfoGameplayUIText.text = value.ToString("D2");
    }

    private void SetRoundInfoGameplayUIText(int value)
    {
        int roundCount = GameManager.Instance.Get<StageService>().GetRoundCount();
        _roundInfoGameplayUIText.text = value.ToString("D2") +" / "+ roundCount.ToString("D2");
    }

    private void SetPlayerLivesInfoGameplayUI()
    {
        int currentLives = GameManager.Instance.Get<PlayerLevelService>().Lives;
        int maxLives = GameManager.Instance.Get<StageService>().GetCurrentStageData().maxPlayerLives;
        _playerLivesInfoGameplayUIText.text = currentLives.ToString() + " / " + maxLives.ToString();
    }

    private void SetStageDifficultyOnSelectionUI(StageDifficultyEnum difficultyEnumValue)
    {
        int difficulty = (int)difficultyEnumValue;

        for (int index = 0; index < _difficultyImageList.Count; index++ )
        { 
            if (index < difficulty)
            {
                _difficultyImageList[index].gameObject.SetActive(true);
            }
            else
            {
                _difficultyImageList[index].gameObject.SetActive(false);
            }
        }
    }

    private void SetStageRecommendedLevelOnSelectionUI(int value)
    {
        _recommendedLevelText.text = value.ToString("D2");
    }

    private void SetStageRecommendedElementsOnSelectionUI(StageSelectionCardUIView stage)
    {
        List<UnitElementEnum> recommendedElements = stage.RecommendedElements;
        IconService iconServiceObj = GameManager.Instance.Get<IconService>();
        UnitColorService colorServiceObj = GameManager.Instance.Get<UnitColorService>();

        for (int index = 0; index < recommendedElements.Count; index++)
        {
            _recommendedElementList[index].elementIcon.sprite = iconServiceObj.GetElementIcon(recommendedElements[index]);
            _recommendedElementList[index].elementIconContainer.color = colorServiceObj.GetElementColor(recommendedElements[index]);
            _recommendedElementList[index].elementNameText.text = recommendedElements[index].ToString();
        }
    }

    private void OnGameplayStateChanged(object[] parameters)
    {
        GameplayStateEnum currentGameplayState = (GameplayStateEnum)parameters[0];

        if (currentGameplayState == GameplayStateEnum.Preparation)
        {
            ToggleEnterCombatButtonVisibility(true);
            ToggleGameplayBottomControlPanel(true);
        }
        else if (currentGameplayState == GameplayStateEnum.Combat)
        {
            HideShopPanel();
            ToggleEnterCombatButtonVisibility(false);
            ToggleGameplayBottomControlPanel(false);
        }
    }

    private void ToggleEnterCombatButtonVisibility(bool value)
    {
        _enterCombatButton.gameObject.SetActive(value);
    }

    private void ToggleGameplayBottomControlPanel(bool value)
    {
        _bottomControlPanel.gameObject.SetActive(value);
    }

    private void HideShopPanel()
    {
        _shopPanel.gameObject.SetActive(false);
    }

    public void StartFlashing()
    {
        if (_flashCoroutine == null)
        {
            _flashCoroutine = StartCoroutine(FlashCoroutine());
        }
    }

    public void StopFlashing()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;

            Color color = _flashingGraphic.color;
            color.a = 1f;
            _flashingGraphic.color = color;
        }
    }

    IEnumerator FlashCoroutine()
    {
        while (true)
        {
            yield return FadeCoroutine(1f, 0f);
            yield return FadeCoroutine(0f, 1f);
        }
    }

    IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        Color color = _flashingGraphic.color;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);
            _flashingGraphic.color = color;
            yield return null;
        }

        color.a = endAlpha;
        _flashingGraphic.color = color;
    }

    private void OnAudioSettingsButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        ToggleAudioSettingsUI(true);
        ToggleMainMenuUIContainer(false);
        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(false);
    }

    private void OnCloseAudioSettingsButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        ToggleAudioSettingsUI(false);
        ToggleMainMenuUIContainer(true);
        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(true);
    }

    private void OnHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        if (_videoInstructionServiceObj == null || _videoInstructionServiceObj.Count == 0)
        {
            Debug.LogWarning("No tutorials found.");
            return;
        }

        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(false);
        ToggleHowToPlayUI(true);
        ToggleMainMenuUIContainer(false);
        _currentInstructionIndex = 0;
        _totalInstructionCount = _videoInstructionServiceObj.Count;
        _totalInstructionCountText.text = _totalInstructionCount.ToString("D2");
        DisplayTutorial(_currentInstructionIndex);
    }

    private void OnCloseHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        if (_videoPlayer.isPlaying)
            _videoPlayer.Stop();

        _videoPlayer.clip = null;

        _instructionTitleText.text = string.Empty;
        _instructionDescriptionText.text = string.Empty;

        ToggleHowToPlayUI(false);
        ToggleMainMenuUIContainer(true);
        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(true);
    }

    private void OnCreditsButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        ToggleCreditsUI(true);
        ToggleMainMenuUIContainer(false);
        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(false);
    }

    private void OnCloseCreditsButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        ToggleCreditsUI(false);
        ToggleMainMenuUIContainer(true);
        PostProcessingManager.Instance.ToggleFullscreenVornoiEffect(true);
    }

    private void ToggleAudioSettingsUI(bool value)
    {
        _audioSettingsUIContainer.SetActive(value);
    }

    private void ToggleHowToPlayUI(bool value)
    {
        _howToPlayUIContainer.SetActive(value);
    }

    private void ToggleCreditsUI(bool value)
    {
        _creditsUIContainer.SetActive(value);
    }

    private void OnExitGameButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        Application.Quit();
    }

    private void DisplayTutorial(int index)
    {
        VideoInstructionEntry videoInstructionEntry = _videoInstructionServiceObj.GetVideoInstructionEntry(index);

        _currentInstructionIndexText.text = (index+1).ToString("D2");
        _instructionTitleText.text = videoInstructionEntry.title;
        _instructionDescriptionText.text = videoInstructionEntry.instruction;

        _videoPlayer.Stop();
        _videoPlayer.clip = videoInstructionEntry.videoClip;

        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        _videoPlayer.Prepare();

        _previousButton.interactable = index > 0;
        _nextButton.interactable = index < _totalInstructionCount - 1;
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        player.prepareCompleted -= OnVideoPrepared;
        player.Play();
    }

    private void OnNextButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        if (_currentInstructionIndex >= _totalInstructionCount - 1)
            return;

        _currentInstructionIndex++;
        DisplayTutorial(_currentInstructionIndex);
    }

    private void OnPreviousButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);
        if (_currentInstructionIndex <= 0)
            return;

        _currentInstructionIndex--;
        DisplayTutorial(_currentInstructionIndex);
    }

    private void InitializeAudioUI()
    {
        foreach (var audioControl in _audioControls)
        {
            float value = AudioManager.Instance.GetVolume(audioControl.channel);

            audioControl.slider.SetValueWithoutNotify(value);
            audioControl.previousVolume = value;

            bool muted = value <= 0.001f;

            audioControl.muteToggle.SetIsOnWithoutNotify(muted);

            UpdateAudioVolumeLabel(audioControl, value);
            UpdateMuteIcon(audioControl, muted);

            audioControl.slider.onValueChanged.RemoveAllListeners();
            audioControl.muteToggle.onValueChanged.RemoveAllListeners();

            audioControl.slider.onValueChanged.AddListener(volume =>
            {
                AudioManager.Instance.SetVolume(audioControl.channel, volume);

                if (volume > 0f)
                    audioControl.previousVolume = volume;

                bool isMuted = volume <= 0.001f;

                audioControl.muteToggle.SetIsOnWithoutNotify(isMuted);

                UpdateAudioVolumeLabel(audioControl, volume);
                UpdateMuteIcon(audioControl, isMuted);
            });

            audioControl.muteToggle.onValueChanged.AddListener(isMuted =>
            {
                AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.ButtonClick);

                if (isMuted)
                {
                    if (audioControl.slider.value > 0f)
                        audioControl.previousVolume = audioControl.slider.value;

                    audioControl.slider.SetValueWithoutNotify(0f);

                    AudioManager.Instance.SetVolume(audioControl.channel, 0f);

                    UpdateAudioVolumeLabel(audioControl, 0f);
                    UpdateMuteIcon(audioControl, true);
                }
                else
                {
                    float restore = Mathf.Max(audioControl.previousVolume, 0.5f);

                    audioControl.slider.SetValueWithoutNotify(restore);

                    AudioManager.Instance.SetVolume(audioControl.channel, restore);

                    UpdateAudioVolumeLabel(audioControl, restore);
                    UpdateMuteIcon(audioControl, false);
                }
            });
        }
    }

    private void UpdateAudioVolumeLabel(AudioControlUI control, float value)
    {
        if (control.valueText != null)
            control.valueText.text = $"{Mathf.RoundToInt(value * 10)}";
    }

    private void UpdateMuteIcon(AudioControlUI control, bool muted)
    {
        Image icon = control.muteIcon;

        if (icon == null)
            return;

        icon.sprite = muted ? _mutedIconSprite : _unmutedIconSprite;
    }
}
