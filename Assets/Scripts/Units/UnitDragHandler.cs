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
        HighlightTileUnderPointer(eventData);
        HighlightInventoryDropZone(eventData);
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
        var go = eventData.pointerCurrentRaycast.gameObject;
        if (go == null) return null;

        Tile tile = go.GetComponent<Tile>() ?? go.GetComponentInParent<Tile>();
        return tile?.Node;
    }

    private void HighlightTileUnderPointer(PointerEventData eventData)
    {
        var go = eventData.pointerCurrentRaycast.gameObject;
        if (go == null) return;

        Tile tile = go.GetComponent<Tile>() ?? go.GetComponentInParent<Tile>();
        if (_highlightedTile == tile) return;

        ClearHighlightedTile();

        if (tile == null) return;

        bool valid = !tile.Node.IsOccupied;
        tile.SetHighlight(true, valid);
        _highlightedTile = tile;
    }

    private void ClearHighlightedTile()
    {
        if (_highlightedTile != null)
        {
            _highlightedTile.SetHighlight(false, false);
            _highlightedTile = null;
        }
    }

    private void HighlightInventoryDropZone(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null) return;

        InventoryDropZoneManager dropZone = eventData.pointerCurrentRaycast.gameObject
            .GetComponent<InventoryDropZoneManager>() ??
            eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventoryDropZoneManager>();

        if (dropZone != null)
        {
            dropZone.SetHighlight(true);
        }
        else
        {
            ClearInventoryHighlight();
        }
    }

    private void ClearInventoryHighlight()
    {
        var zones = FindObjectsOfType<InventoryDropZoneManager>();
        foreach (var zone in zones)
        {
            zone.SetHighlight(false);
        }
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
