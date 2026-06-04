using AutoBattler.Event;
using AutoBattler.Main;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUnitCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFaction;
    [SerializeField] private Image _unitType;
    [SerializeField] private TMP_Text _unitLevelText;

    public UnitData UnitData { get; private set; }

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private LayoutElement _layoutElement;
    private RectTransform _cardContainer;

    private GameObject _placeholder;
    private Image _dragSprite;
    private Tile _highlightedTile;
    private bool _isOutsideContainer;
    private int _originalSiblingIndex;

    private TeamService _teamServiceObj;

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

        UnitData = unitData;
        _canvas = canvas;
        _cardContainer = cardContainer;

        SetupCardData();
    }

    private void SetupCardData()
    {
        _unitIcon.sprite = UnitData.unitIcon;
        _unitLevelText.text = UnitData.unitLevel.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalSiblingIndex = transform.GetSiblingIndex();
        _isOutsideContainer = false;
        _placeholder = new GameObject("Placeholder");
        _placeholder.transform.SetParent(_cardContainer);
        _placeholder.transform.SetSiblingIndex(_originalSiblingIndex);

        LayoutElement placeholderLayout = _placeholder.AddComponent<LayoutElement>();
        placeholderLayout.preferredWidth = _layoutElement.preferredWidth;
        placeholderLayout.preferredHeight = _layoutElement.preferredHeight;

        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.85f;

        EventBusManager.Instance.Raise(EventNameEnum.ToggleDiscardDropZone, true);
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
            ClearHighlightedTile();
        }
        else
        {
            HandleOutsideDrag(eventData);

            Tile tile = GetTileUnderPointer(eventData);

            if (tile != null)
            {
                HighlightTileUnderPointer(tile);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        DiscardUnitDropZoneManager discardUnitDropZone = null;
        bool spawned = false;

        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        if (_isOutsideContainer)
        {
            if (target != null)
            {
                discardUnitDropZone = target.GetComponentInParent<DiscardUnitDropZoneManager>();

                if (discardUnitDropZone != null)
                {
                    discardUnitDropZone.HandleInventoryDrop(this);
                    CleanupAfterDrag();
                    RaiseToggleDiscardDropZoneEvent();
                    return;
                }
            }

            if (discardUnitDropZone == null)
            {
                Tile tile = GetTileUnderPointer(eventData);

                if (tile != null)
                {
                    spawned = TrySpawnUnitOnTile(tile);
                }
            }
        }

        if (!spawned)
        {
            transform.SetParent(_cardContainer);
            transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
        }

        CleanupAfterDrag();
        RaiseReorderInventoryLayoutEvent();
        RaiseToggleDiscardDropZoneEvent();
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

    private Tile GetTileUnderPointer(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);

        if (hit.collider == null)
        {
            return null;
        }
        else
        {
            if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile tile))
            {
                return tile;
            }
            else
            {
                return null;
            }
        }
    }

    private bool TrySpawnUnitOnTile(Tile tile)
    {
        if (GameplayManager.Instance.CurrentGameplayState != GameplayStateEnum.Preparation) return false;

        if (!_teamServiceObj.CanAddUnitToField(TeamEnum.Team1)) return false;

        if (tile == null || tile.Node == null || tile.Node.IsOccupied)
        {
            return false;
        }
        else
        {
            GameplayManager.Instance.DeployUnit(this, tile.Node, TeamEnum.Team1);
            return true;
        }
    }

    private void HighlightTileUnderPointer(Tile tile)
    {
        if (tile == null) return;
        if (_highlightedTile == tile) return;

        ClearHighlightedTile();

        bool isValid = GameplayManager.Instance.CurrentGameplayState == GameplayStateEnum.Preparation &&
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

    private void CleanupDragSprite()
    {
        if (_dragSprite != null)
        {
            Destroy(_dragSprite.gameObject);
            _dragSprite = null;
        }
    }

    private void CleanupPlaceholder()
    {
        if (_placeholder != null)
        {
            Destroy(_placeholder);
            _placeholder = null;
        }
    }

    private void CleanupAfterDrag()
    {
        CleanupPlaceholder();
        CleanupDragSprite();
        ClearHighlightedTile();
    }

    private void RaiseReorderInventoryLayoutEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.ReorderInventoryLayout, false);
    }

    private void RaiseToggleDiscardDropZoneEvent()
    {
        EventBusManager.Instance.Raise(EventNameEnum.ToggleDiscardDropZone, false);
    }

    public void Reset()
    {
        UnitData = default;
        _unitIcon.sprite = null;
        _unitFaction.sprite = null;
        _unitType.sprite = null;
        _unitLevelText.text = string.Empty;
    }
}
