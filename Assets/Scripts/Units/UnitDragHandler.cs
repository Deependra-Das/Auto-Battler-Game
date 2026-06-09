using AutoBattler.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BaseUnit _unit;
    private Collider2D _unitCollider;
    private Camera _mainCamera;

    private Canvas _canvas;
    private RectTransform _canvasRect;

    private GameObject _dragSprite;
    private Vector3 _dragOffset;
    private Tile _hoveredTile;
    private InventoryDropZoneManager _hoveredInventoryDropZone;
    private DiscardUnitDropZoneManager _hoveredDiscardDropZone;
    private Node _originalNode;
    private bool isDragging;


    void Awake()
    {
        _unit = GetComponent<BaseUnit>();
        _unitCollider = GetComponent<Collider2D>();
        _mainCamera = Camera.main;

        _canvas = UIManager.Instance.UICanvas;
        _canvasRect = UIManager.Instance.CanvasRect;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_unit.CanBeDragged) return;

        isDragging = true;

        _originalNode = _unit.CurrentNode;
        _unit.ReleaseCurrentNode();

        _unitCollider.enabled = false;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _dragOffset = transform.position - worldPos;

        ClearDiscardUnitDropZoneHighlight();
        ClearInventoryDropZoneHighlight();
        RaiseToggleInventoryDropZoneEvent(true);
        RaiseToggleDiscardDropZoneEvent(true);
        CreateDragSprite();
        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        UpdateDragPosition(eventData);

        Tile tile = TryGetTileUnderPointer(eventData);

        if (tile != null)
        {
            ClearDiscardUnitDropZoneHighlight();
            ClearInventoryDropZoneHighlight();
            HighlightTileUnderPointer(tile);
            return;
        }

        DiscardUnitDropZoneManager discardZone = TryGetDiscardZoneUnderPointer(eventData);

        if (discardZone != null)
        {
            ClearHighlightedTile();
            ClearInventoryDropZoneHighlight();
            HighlightDiscardUnitDropZone(discardZone);
            return;
        }

        InventoryDropZoneManager inventoryDropZone = TryGetInventoryDropZoneUnderPointer(eventData);

        if (inventoryDropZone != null)
        {
            ClearHighlightedTile();
            ClearDiscardUnitDropZoneHighlight();
            HighlightInventoryDropZone(inventoryDropZone);
            return;
        }

        ClearHighlightedTile();
        ClearInventoryDropZoneHighlight();
        ClearDiscardUnitDropZoneHighlight();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        _unitCollider.enabled = true;

        DiscardUnitDropZoneManager discardZone = TryGetDiscardZoneUnderPointer(eventData);

        if(discardZone!=null)
        {
            discardZone.HandleUnitDrop(this.GetComponent<BaseUnit>());
            CleanupAfterDrag();
            return;
        }

        InventoryDropZoneManager inventoryZone = TryGetInventoryDropZoneUnderPointer(eventData);

        if (inventoryZone != null)
        {
            inventoryZone.HandleUnitDrop(this.GetComponent<BaseUnit>());
            CleanupAfterDrag();
            return;
        }

        Node targetNode = GetNodeUnderPointer(eventData);

        if (targetNode != null && !targetNode.IsOccupied)
            _unit.SnapToNode(targetNode);
        else
            _unit.OnDragCancelled(_originalNode);

        CleanupAfterDrag();  
    }

    private Tile TryGetTileUnderPointer(PointerEventData eventData)
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

    private DiscardUnitDropZoneManager TryGetDiscardZoneUnderPointer(PointerEventData eventData)
    {
        var raycastObj = eventData.pointerCurrentRaycast.gameObject;

        if (raycastObj == null)
            return null;

        return raycastObj.GetComponentInParent<DiscardUnitDropZoneManager>();
    }

    private InventoryDropZoneManager TryGetInventoryDropZoneUnderPointer(PointerEventData eventData)
    {
        var raycastObj = eventData.pointerCurrentRaycast.gameObject;

        if (raycastObj == null)
            return null;

        return raycastObj.GetComponentInParent<InventoryDropZoneManager>();
    }

    private void CreateDragSprite()
    {
        _dragSprite = new GameObject("DragUnitSprite");
        _dragSprite.transform.SetParent(_canvas.transform, false);

        Image spriteImage = _dragSprite.AddComponent<Image>();
        spriteImage.sprite = GetComponent<SpriteRenderer>().sprite;
        spriteImage.SetNativeSize();
        spriteImage.raycastTarget = false;

        _unit.UpdateUnitSpriteVisibility(false);
        _unit.UpdateUnitUIVisibility(false);
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (_dragSprite == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint);

        _dragSprite.GetComponent<RectTransform>().localPosition = localPoint;
    }

    private Node GetNodeUnderPointer(PointerEventData eventData)
    {
        var tile_GO = eventData.pointerCurrentRaycast.gameObject;
        if (tile_GO == null) return null;

        Tile tile = tile_GO.GetComponent<Tile>() ?? tile_GO.GetComponentInParent<Tile>();
        return tile?.Node;
    }

    private void HighlightTileUnderPointer(Tile tile)
    {
        if (tile == null) return;
        if (_hoveredTile == tile) return;

        ClearHighlightedTile();

        bool valid = !tile.Node.IsOccupied;
        tile.OnInteractShowHighlight();
        _hoveredTile = tile;
    }

    private void ClearHighlightedTile()
    {
        if (_hoveredTile != null)
        {
            _hoveredTile.HideHighlight();
            _hoveredTile = null;
        }
    }

    private void HighlightInventoryDropZone(InventoryDropZoneManager inventoryDropZone)
    {
        if (_hoveredInventoryDropZone == inventoryDropZone) return;

        ClearInventoryDropZoneHighlight();

        _hoveredInventoryDropZone = inventoryDropZone;
        _hoveredInventoryDropZone.ShowInventoryHighlight();
    }

    private void ClearInventoryDropZoneHighlight()
    {
        if (_hoveredInventoryDropZone == null) return;

        _hoveredInventoryDropZone.HideInventoryHighlight();
        _hoveredInventoryDropZone = null;
    }
    private void HighlightDiscardUnitDropZone(DiscardUnitDropZoneManager discardUnitDropZone)
    {
        if (_hoveredDiscardDropZone == discardUnitDropZone) return;

        ClearDiscardUnitDropZoneHighlight();

        _hoveredDiscardDropZone = discardUnitDropZone;
        _hoveredDiscardDropZone.ShowDiscardDropZoneHighlight();
    }

    private void ClearDiscardUnitDropZoneHighlight()
    {
        if (_hoveredDiscardDropZone == null) return;

        _hoveredDiscardDropZone.HideDiscardDropZoneHighlight();
        _hoveredDiscardDropZone = null;
    }

    private void CleanupAfterDrag()
    {
        if (_dragSprite != null)
            Destroy(_dragSprite);

        _unit.UpdateUnitSpriteVisibility(true);
        _unit.UpdateUnitUIVisibility(true);

        ClearHighlightedTile();
        ClearInventoryDropZoneHighlight();
        ClearDiscardUnitDropZoneHighlight();
        RaiseToggleInventoryDropZoneEvent(false);
        RaiseToggleDiscardDropZoneEvent(false);
    }

    protected Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 pos = screenPosition;
        pos.z = Mathf.Abs(_mainCamera.transform.position.z);
        return _mainCamera.ScreenToWorldPoint(pos);
    }

    private void RaiseToggleInventoryDropZoneEvent(bool state)
    {
        EventBusManager.Instance.Raise(EventNameEnum.ToggleInventoryDropZone, state);
    }
    private void RaiseToggleDiscardDropZoneEvent(bool state)
    {
        EventBusManager.Instance.Raise(EventNameEnum.ToggleDiscardDropZone, state);
    }
}
