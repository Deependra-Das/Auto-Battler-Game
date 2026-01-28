using System.Collections;
using System.Collections.Generic;
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
    private bool _droppedOnValidDropZone;

    private Node _originalNode;
    private Tile _highlightedTile;
    private bool isDragging;
    private InventoryDropZoneManager _highlightInventoryPanel;

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

        _droppedOnValidDropZone = false;
        isDragging = true;

        _originalNode = _unit.CurrentNode;
        _unit.TemporarilyReleaseNode();

        _unitCollider.enabled = false;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _dragOffset = transform.position - worldPos;

        CreateDragSprite();
        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        UpdateDragPosition(eventData);

        var gameObject = eventData.pointerCurrentRaycast.gameObject;

        if (!gameObject) return;

        if (gameObject.TryGetComponent<Tile>(out Tile tile))
        {
            HighlightTileUnderPointer(tile);
        }
        else if (gameObject.TryGetComponent<InventoryDropZoneManager>(out InventoryDropZoneManager inventoryDropZone))
        {
            HighlightInventoryDropZone(inventoryDropZone);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        _unitCollider.enabled = true;

        ClearHighlightedTile();
        ClearInventoryHighlight();

        if (_droppedOnValidDropZone)
        {
            _unit.OnDragCompleted();
            return;
        }

        Node targetNode = GetNodeUnderPointer(eventData);
        if (targetNode != null && !targetNode.IsOccupied)
            _unit.SnapToNode(targetNode);
        else
            _unit.OnDragCancelled(_originalNode);

        CleanupAfterDrag();
    }

    public void MarkDroppedOnValidZone()
    {
        _droppedOnValidDropZone = true;
        ClearHighlightedTile();
        ClearInventoryHighlight();
        CleanupAfterDrag();
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
        if (_highlightedTile == tile) return;

        ClearHighlightedTile();

        if (tile == null) return;

        bool valid = !tile.Node.IsOccupied;
        tile.OnInteractSetHighlight(true, valid);
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

    private void HighlightInventoryDropZone(InventoryDropZoneManager inventoryDropZone)
    {
        if (_highlightInventoryPanel == inventoryDropZone) return;

        ClearInventoryHighlight();

        _highlightInventoryPanel = inventoryDropZone;
        inventoryDropZone.OnInteractSetHighlight(true);
    }

    private void ClearInventoryHighlight()
    {
        if (_highlightInventoryPanel == null) return;

        _highlightInventoryPanel.OnInteractSetHighlight(false);
        _highlightInventoryPanel = null;
    }

    private void CleanupAfterDrag()
    {
        if (_dragSprite != null)
            Destroy(_dragSprite);

        _unit.UpdateUnitSpriteVisibility(true);
        _unit.UpdateUnitUIVisibility(true);
    }

    protected Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 pos = screenPosition;
        pos.z = Mathf.Abs(_mainCamera.transform.position.z);
        return _mainCamera.ScreenToWorldPoint(pos);
    }
}
