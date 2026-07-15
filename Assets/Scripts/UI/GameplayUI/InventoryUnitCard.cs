using AutoBattler.Event;
using AutoBattler.Main;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InventoryUnitCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFactionIcon;
    [SerializeField] private Image _unitTypeIcon;
    [SerializeField] private Image _unitElementIcon;
    [SerializeField] private Image _unitElementIconContainer;
    [SerializeField] private Image _overlay;
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _unitLevelText;

    public UnitData UnitData { get; private set; }

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private LayoutElement _layoutElement;
    private RectTransform _cardContainer;

    private GameObject _placeholder;
    private Image _dragSprite;
    private Node _hoveredNode;
    private DiscardUnitDropZoneManager _hoveredDiscardDropZone;
    private bool _isOutsideContainer;
    private int _originalSiblingIndex;

    private TeamService _teamServiceObj;
    private DragVisualPoolService _dragVisualPoolServiceObj;
    private TileGridService _tileGridServiceObj;
    private HighlightTileService _highlightTileServiceObj;
    private GraphService _graphServiceObj;
    private IconService _unitIconServiceObj;
    private UnitColorService _unitColorServiceObj;


    private void Awake()
    {
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
    }

    private void OnDestroy()
    {
        if (_placeholder != null)
            Destroy(_placeholder);

        CleanupDragSprite();
    }

    public void Initialize(UnitData unitData, Canvas canvas, RectTransform cardContainer)
    {
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _dragVisualPoolServiceObj = GameManager.Instance.Get<DragVisualPoolService>();
        _tileGridServiceObj = GameManager.Instance.Get<TileGridService>();
        _highlightTileServiceObj = GameManager.Instance.Get<HighlightTileService>();
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
        _unitIconServiceObj = GameManager.Instance.Get<IconService>();
        _unitColorServiceObj = GameManager.Instance.Get<UnitColorService>();

        UnitData = unitData;
        _canvas = canvas;
        _cardContainer = cardContainer;

        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = UnitData.unitIcon;
        _unitNameText.text = UnitData.unitName.ToString();
        _unitLevelText.text = UnitData.unitLevel.ToString();
        _unitFactionIcon.sprite = _unitIconServiceObj.GetFactionIcon(UnitData.unitFaction);
        _unitTypeIcon.sprite = _unitIconServiceObj.GetUnitTypeIcon(UnitData.unitType);
        _unitElementIcon.sprite = _unitIconServiceObj.GetElementIcon(UnitData.unitElement);
        Color elementColor = _unitColorServiceObj.GetElementColor(UnitData.unitElement);
        _unitElementIconContainer.color = elementColor;
        _overlay.color = elementColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalSiblingIndex = transform.GetSiblingIndex();
        _isOutsideContainer = false;
        _placeholder = _dragVisualPoolServiceObj.GetPlaceholder(_cardContainer, _originalSiblingIndex, _layoutElement.preferredWidth, _layoutElement.preferredHeight);

        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.85f;

        ClearDiscardUnitDropZoneHighlight();
        RaiseToggleDiscardDropZoneEvent(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        bool isInsideContainer = RectTransformUtility.RectangleContainsScreenPoint(
            _cardContainer, eventData.position, eventData.pressEventCamera);

        if (isInsideContainer)
        {
            HandleReorder();
            CleanupDragSprite();
            ClearHighlightedTileNode();
            ClearDiscardUnitDropZoneHighlight();
            return;
        }
        
        HandleOutsideDrag(eventData);

        Node node = TryGetNodeUnderPointer(eventData);

        if (node != null)
        {
            ClearDiscardUnitDropZoneHighlight();
            HighlightTileAtNode(node);
            return;
        }

        DiscardUnitDropZoneManager discardZone = TryGetDiscardZoneUnderPointer(eventData);

        if (discardZone != null)
        {
            ClearHighlightedTileNode();
            HighlightDiscardUnitDropZone(discardZone);
            return;
        }

        ClearHighlightedTileNode();
        ClearDiscardUnitDropZoneHighlight();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        bool spawned = false;

        if (_isOutsideContainer)
        {
            DiscardUnitDropZoneManager discardZone = TryGetDiscardZoneUnderPointer(eventData);

            if (discardZone != null)
            {
                discardZone.HandleInventoryDrop(this);
                CleanupAfterDrag();
                RaiseToggleDiscardDropZoneEvent(false);
                return;
            }

            Node node = TryGetNodeUnderPointer(eventData);

            if (node != null)
            {
                spawned = TrySpawnUnitOnNode(node);
            }
        }

        if (!spawned)
        {
            transform.SetParent(_cardContainer);
            transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
        }

        CleanupAfterDrag();
        RaiseReorderInventoryLayoutEvent();
        RaiseToggleDiscardDropZoneEvent(false);
    }

    private void HandleReorder()
    {
        if (_isOutsideContainer)
        {
            _isOutsideContainer = false;
            _canvasGroup.alpha = 0.85f;
        }

        int targetIndex = _cardContainer.childCount;

        for (int i = 0; i < _cardContainer.childCount; i++)
        {
            if (transform.position.x < _cardContainer.GetChild(i).position.x)
            {
                targetIndex = i;
                break;
            }
        }

        if (_placeholder != null)
        {
            _placeholder.transform.SetSiblingIndex(targetIndex);
        }
    }

    private void HandleOutsideDrag(PointerEventData eventData)
    {
        if (!_isOutsideContainer)
        {
            _isOutsideContainer = true;
            _canvasGroup.alpha = 0f;

            if (_unitIcon != null && _dragSprite == null)
            {
                _dragSprite = _dragVisualPoolServiceObj.GetDragSprite(_unitIcon.sprite);
            }
        }

        if (_dragSprite != null)
            _dragSprite.transform.position = eventData.position;
    }

    private Node TryGetNodeUnderPointer(PointerEventData eventData)
    {
        Tilemap tilemap = _tileGridServiceObj.CurrentTileMap;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        worldPos.z = 0;

        Vector3Int cell = tilemap.WorldToCell(worldPos);

        if (!tilemap.HasTile(cell))
            return null;

        _graphServiceObj.TryGetNodeAtCell(cell, out Node node);

        return node;
    }

    private DiscardUnitDropZoneManager TryGetDiscardZoneUnderPointer(PointerEventData eventData)
    {
        var raycastObj = eventData.pointerCurrentRaycast.gameObject;

        if (raycastObj == null)
            return null;

        return raycastObj.GetComponentInParent<DiscardUnitDropZoneManager>();
    }

    private bool TrySpawnUnitOnNode(Node node)
    {
        if (GameplayManager.Instance.CurrentGameplayState != GameplayStateEnum.Preparation)
            return false;

        if (!_teamServiceObj.CanAddUnitToField(TeamEnum.Team1))
            return false;

        if (node == null || node.IsOccupied)
            return false;

        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.PlaceUnitOnField);
        GameplayManager.Instance.DeployUnit(this, node, TeamEnum.Team1);

        return true;
    }

    private void HighlightTileAtNode(Node node)
    {
        if (node == null)
            return;

        if (_hoveredNode == node)
            return;

        _hoveredNode = node;

        bool valid = GameplayManager.Instance.CurrentGameplayState == GameplayStateEnum.Preparation && !node.IsOccupied;

        _highlightTileServiceObj.ShowTileHighlight(node.worldPosition, valid);
    }

    private void ClearHighlightedTileNode()
    {
        _hoveredNode = null;
        _highlightTileServiceObj.HideTileHighlight();
    }

    private void HighlightDiscardUnitDropZone(DiscardUnitDropZoneManager discardUnitDropZone)
    {
        if (_hoveredDiscardDropZone == discardUnitDropZone) return;

        ClearDiscardUnitDropZoneHighlight();

        _hoveredDiscardDropZone = discardUnitDropZone;
        _hoveredDiscardDropZone.ShowDiscardDropZoneHighlight();
        _hoveredDiscardDropZone.ShowRefundAmount(UnitData.baseUnitCost);
    }

    private void ClearDiscardUnitDropZoneHighlight()
    {
        if (_hoveredDiscardDropZone == null) return;

        _hoveredDiscardDropZone.HideDiscardDropZoneHighlight();
        _hoveredDiscardDropZone.RevertRefundAmount();
        _hoveredDiscardDropZone = null;
    }

    private void CleanupDragSprite()
    {
        if (_dragSprite != null)
        {
            _dragVisualPoolServiceObj.ReleaseDragSprite();
            _dragSprite = null;
        }
    }

    private void CleanupPlaceholder()
    {
        if (_placeholder != null)
        {
            _dragVisualPoolServiceObj.ReleasePlaceholder();
            _placeholder = null;
        }
    }

    private void CleanupAfterDrag()
    {
        CleanupPlaceholder();
        CleanupDragSprite();
        ClearHighlightedTileNode();
        ClearDiscardUnitDropZoneHighlight();
    }

    private void RaiseReorderInventoryLayoutEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.ReorderInventoryLayout, false);
    }

    private void RaiseToggleDiscardDropZoneEvent(bool state)
    {
        EventBusManager.Instance.Raise(EventNameEnum.ToggleDiscardDropZone, state);
    }

    public void Reset()
    {
        UnitData = default;
        _unitIcon.sprite = null;
        _unitFactionIcon.sprite = null;
        _unitTypeIcon.sprite = null;
        _unitElementIcon.sprite = null;
        _unitNameText.text = string.Empty;
        _unitLevelText.text = string.Empty;
    }
}
