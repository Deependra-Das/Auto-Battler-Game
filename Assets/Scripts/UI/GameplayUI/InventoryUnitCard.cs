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

    private bool _isOutsideContainer;
    private int _originalSiblingIndex;

    private Image _ghostIcon;
    private InventoryService _inventoryService;

    private void Awake()
    {
        _inventoryService = GameManager.Instance.Get<InventoryService>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        _cardContainer = transform.parent as RectTransform;
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

        _placeholder = new GameObject("Placeholder");
        _placeholder.transform.SetParent(_cardContainer);
        _placeholder.transform.SetSiblingIndex(_originalSiblingIndex);

        LayoutElement placeholderLayout = _placeholder.AddComponent<LayoutElement>();
        placeholderLayout.preferredWidth = _layoutElement.preferredWidth;
        placeholderLayout.preferredHeight = _layoutElement.preferredHeight;

        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.85f;

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
            DestroyGhostIcon();
        }
        else
        {
            HandleOutsideDrag(eventData);
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

        _placeholder.transform.SetSiblingIndex(targetIndex);
    }

    private void HandleOutsideDrag(PointerEventData eventData)
    {
        if (!_isOutsideContainer)
        {
            _isOutsideContainer = true;
            _canvasGroup.alpha = 0f;

            if (_unitIcon != null && _ghostIcon == null)
            {
                GameObject ghostGO = new GameObject("GhostIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                ghostGO.transform.SetParent(_canvas.transform);
                ghostGO.transform.SetAsLastSibling();

                _ghostIcon = ghostGO.GetComponent<Image>();
                _ghostIcon.sprite = _unitIcon.sprite;
                _ghostIcon.SetNativeSize();
                _ghostIcon.raycastTarget = false;
            }
        }

        if (_ghostIcon != null)
            _ghostIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        bool spawned = false;

        if (_isOutsideContainer)
        {
            spawned = TrySpawnUnitOnTile(eventData);
        }

        if (!spawned)
        {
            transform.SetParent(_cardContainer);
            transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
        }

        Destroy(_placeholder);
        DestroyGhostIcon();

        UIManager.Instance.RefreshInventoryOrder();
    }

    private void DestroyGhostIcon()
    {
        if (_ghostIcon != null)
        {
            Destroy(_ghostIcon.gameObject);
            _ghostIcon = null;
        }
    }

    private bool TrySpawnUnitOnTile(PointerEventData eventData)
    {
        if (GameplayManager.Instance.CurrentState != GameplayStateEnum.Preparation)
            return false;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
        if (hit.collider == null)
            return false;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null || tile.Node == null || tile.Node.IsOccupied)
            return false;

        _inventoryService.DeployUnit(this, tile.Node);

        return true;
    }
}
