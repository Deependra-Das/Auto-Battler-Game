using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUnitCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFaction;
    [SerializeField] private Image _unitType;
    [SerializeField] private Image _unitLevel;

    public UnitData UnitData { get; private set; }

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private LayoutElement _layoutElement;

    private RectTransform _cardContainer;
    private GameObject _placeholder;
    private Tile _highlightedTile;
    private bool _isOutsideContainer;
    private int _originalSiblingIndex;
    private DiscardUnitDropZoneManager _highlightdiscardUnitPanel;
    private bool _droppedOnDiscardUnitZone;
    private Image _dragSprite;
    private InventoryService _inventoryService;

    private void Awake()
    {
        _inventoryService = GameManager.Instance.Get<InventoryService>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        _cardContainer = transform.parent as RectTransform;
    }

    private void OnDestroy()
    {
        if (_placeholder != null)
            Destroy(_placeholder);

        CleanupDragSprite();
    }

    public void Initialize(UnitData unitData, Canvas canvas)
    {
        UnitData = unitData;
        _unitIcon.sprite = unitData.unitIcon;
        _canvas = canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalSiblingIndex = transform.GetSiblingIndex();
        _isOutsideContainer = false;
        _droppedOnDiscardUnitZone = false;
        _placeholder = new GameObject("Placeholder");
        _placeholder.transform.SetParent(_cardContainer);
        _placeholder.transform.SetSiblingIndex(_originalSiblingIndex);

        LayoutElement placeholderLayout = _placeholder.AddComponent<LayoutElement>();
        placeholderLayout.preferredWidth = _layoutElement.preferredWidth;
        placeholderLayout.preferredHeight = _layoutElement.preferredHeight;

        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.85f;

        EventBusManager.Instance.Raise(EventNameEnum.InventoryUnitCardDragged, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        bool isInsideContainer = RectTransformUtility.RectangleContainsScreenPoint(
            _cardContainer,
            eventData.position,
            eventData.pressEventCamera
        );

        if (isInsideContainer)
        {
            HandleReorder();
            CleanupDragSprite();
            ClearHighlightedTile();
        }
        else
        {
            HandleOutsideDrag(eventData);
            var gameObject = GetTileUnderPointer(eventData);

            if (!gameObject) return;

            if (gameObject.TryGetComponent<Tile>(out Tile tile))
            {
                HighlightTileUnderPointer(tile);
            }
            else if (gameObject.TryGetComponent<DiscardUnitDropZoneManager>(out DiscardUnitDropZoneManager discardUnitDropZone))
            {
                HighlightDiscardUnitDropZone(discardUnitDropZone);
            }
        }
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
                GameObject ghostGO = new GameObject("DragSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                ghostGO.transform.SetParent(_canvas.transform);
                ghostGO.transform.SetAsLastSibling();

                _dragSprite = ghostGO.GetComponent<Image>();
                _dragSprite.sprite = _unitIcon.sprite;
                _dragSprite.SetNativeSize();
                _dragSprite.raycastTarget = false;
            }
        }

        if (_dragSprite != null)
            _dragSprite.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        ClearHighlightedTile();
        ClearDiscardUnitDropZoneHighlight();
        bool spawned = false;

        if (_isOutsideContainer && _droppedOnDiscardUnitZone)
        {
            if (_placeholder != null)
            {
                Destroy(_placeholder);
                _placeholder = null;
            }

            CleanupAfterDrag();
            return;
        }

        if (_isOutsideContainer)
        {
            spawned = TrySpawnUnitOnTile(eventData);
        }

        if (!spawned)
        {
            transform.SetParent(_cardContainer);
            transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
        }

        if (_placeholder != null)
        {
            Destroy(_placeholder);
            _placeholder = null;
        }

        CleanupAfterDrag();
    }

    private void CleanupDragSprite()
    {
        if (_dragSprite != null)
        {
            Destroy(_dragSprite.gameObject);
            _dragSprite = null;
        }
    }

    private void CleanupAfterDrag()
    {
        CleanupDragSprite();
        ClearHighlightedTile();
        ClearDiscardUnitDropZoneHighlight();
        DragCompleted();
        UIManager.Instance.RefreshInventoryOrder();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardContainer);
    }

    private bool TrySpawnUnitOnTile(PointerEventData eventData)
    {
        if (GameplayManager.Instance.CurrentState != GameplayStateEnum.Preparation) return false;

        var teamService = GameManager.Instance.Get<TeamService>();

        if (!teamService.CanAddUnitToField(TeamEnum.Team1)) return false;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
        if (hit.collider == null)
            return false;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null || tile.Node == null || tile.Node.IsOccupied)
            return false;

        _inventoryService.DeployUnit(this, tile.Node);
        UIManager.Instance.RemoveInventoryUnitCard(this);
        return true;
    }

    private void HighlightTileUnderPointer(Tile tile)
    {
        if (tile == null) return;
        if (_highlightedTile == tile) return;

        ClearHighlightedTile();

        bool isValid = GameplayManager.Instance.CurrentState == GameplayStateEnum.Preparation &&
            tile.Node != null && !tile.Node.IsOccupied;

        tile.OnInteractSetHighlight(true, isValid);
        _highlightedTile = tile;
    }

    private void ClearHighlightedTile()
    {
        if (_highlightedTile != null)
        {
            _highlightedTile.OnInteractSetHighlight(false, false);
            _highlightedTile = null;
        }
    }

    private GameObject GetTileUnderPointer(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
        if (hit.collider == null)
            return null;

        return hit.collider.gameObject;
    }

    private void HighlightDiscardUnitDropZone(DiscardUnitDropZoneManager discardUnitDropZone)
    {
        if (_highlightdiscardUnitPanel == discardUnitDropZone) return;

        ClearDiscardUnitDropZoneHighlight();

        _highlightdiscardUnitPanel = discardUnitDropZone;
        EventBusManager.Instance.Raise(EventNameEnum.HighlightDiscardPanel, true);
    }

    private void ClearDiscardUnitDropZoneHighlight()
    {
        if (_highlightdiscardUnitPanel == null) return;

        EventBusManager.Instance.Raise(EventNameEnum.HighlightDiscardPanel, false);
        _highlightdiscardUnitPanel = null;
    }

    public void MarkDroppedOnDiscardUnitZone()
    {
        _droppedOnDiscardUnitZone = true;
        CleanupAfterDrag();
    }

    private void DragCompleted()
    {
        EventBusManager.Instance.Raise(EventNameEnum.InventoryUnitCardDragged, false);
    }
}
