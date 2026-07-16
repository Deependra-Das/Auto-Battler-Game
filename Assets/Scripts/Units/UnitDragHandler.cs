using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.VFX;

public class UnitDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VisualEffect _vfxParticleGraph;

    private BaseUnit _unit;
    private Collider2D _unitCollider;
    private Camera _mainCamera;

    private Canvas _canvas;
    private RectTransform _canvasRect;

    private Image _dragSprite;
    private Vector3 _dragOffset;
    private Node _hoveredNode;
    private InventoryDropZoneManager _hoveredInventoryDropZone;
    private DiscardUnitDropZoneManager _hoveredDiscardDropZone;
    private Node _originalNode;
    private bool isDragging;
    private DragVisualPoolService _dragVisualPoolService;
    private TileGridService _tileGridServiceObj;
    private HighlightTileService _highlightTileServiceObj;
    private GraphService _graphServiceObj;
    private RectTransform _dragSpriteRectTransform;

    void Awake()
    {
        _unit = GetComponent<BaseUnit>();
        _unitCollider = GetComponent<Collider2D>();  
    }

    public void Initialize()
    {
        _mainCamera = Camera.main;
        _canvas = UIManager.Instance.UICanvas;
        _canvasRect = UIManager.Instance.CanvasRect;
        _dragVisualPoolService = GameManager.Instance.Get<DragVisualPoolService>();
        _tileGridServiceObj = GameManager.Instance.Get<TileGridService>();
        _highlightTileServiceObj = GameManager.Instance.Get<HighlightTileService>();
        _graphServiceObj = GameManager.Instance.Get<GraphService>();
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
        _vfxParticleGraph.Stop();
        CreateDragSprite();
        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        UpdateDragPosition(eventData);

        Node node = TryGetNodeUnderPointer(eventData);

        if (node != null)
        {
            ClearDiscardUnitDropZoneHighlight();
            ClearInventoryDropZoneHighlight();
            HighlightTileAtNode(node);
            return;
        }

        DiscardUnitDropZoneManager discardZone = TryGetDiscardZoneUnderPointer(eventData);

        if (discardZone != null)
        {
            ClearHighlightedTileNode();
            ClearInventoryDropZoneHighlight();
            HighlightDiscardUnitDropZone(discardZone);
            return;
        }

        InventoryDropZoneManager inventoryDropZone = TryGetInventoryDropZoneUnderPointer(eventData);

        if (inventoryDropZone != null)
        {
            ClearHighlightedTileNode();
            ClearDiscardUnitDropZoneHighlight();
            HighlightInventoryDropZone(inventoryDropZone);
            return;
        }

        ClearHighlightedTileNode();
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

        Node targetNode = TryGetNodeUnderPointer(eventData);

        if (targetNode != null && targetNode.canPlayerDeploy && !targetNode.IsOccupied)
        {
            AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.PlaceUnitOnField);
            _unit.SnapToNode(targetNode);

        _vfxParticleGraph.Stop();

        if (_unit.hasAnyBuff)
        {
            _vfxParticleGraph.Reinit();
            _vfxParticleGraph.Play();
        }
        }
        else
            _unit.OnDragCancelled(_originalNode);

        CleanupAfterDrag();  
    }

    private Node TryGetNodeUnderPointer(PointerEventData eventData)
    {
        Tilemap tilemap = _tileGridServiceObj.CurrentGameplayTileMap;

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

    private InventoryDropZoneManager TryGetInventoryDropZoneUnderPointer(PointerEventData eventData)
    {
        var raycastObj = eventData.pointerCurrentRaycast.gameObject;

        if (raycastObj == null)
            return null;

        return raycastObj.GetComponentInParent<InventoryDropZoneManager>();
    }

    private void CreateDragSprite()
    {
        _dragSprite = _dragVisualPoolService.GetDragSprite(_unit.UnitData.unitIcon);
        _dragSpriteRectTransform = _dragVisualPoolService.GetDragSpriteRectTransform();
        _unit.UpdateUnitSpriteVisibility(false);
        _unit.UpdateUnitUIVisibility(false);
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (_dragSprite == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, eventData.position, 
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint);

        _dragSpriteRectTransform.localPosition = localPoint;
    }

    private void HighlightTileAtNode(Node node)
    {
        if (node == null)
            return;

        if (_hoveredNode == node)
            return;

        _hoveredNode = node;

        bool valid = GameplayManager.Instance.CurrentGameplayState == GameplayStateEnum.Preparation && node.canPlayerDeploy && !node.IsOccupied;

        _highlightTileServiceObj.ShowTileHighlight(node.worldPosition, valid);
    }

    private void ClearHighlightedTileNode()
    {
        _hoveredNode = null;
        _highlightTileServiceObj.HideTileHighlight();
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
        _hoveredDiscardDropZone.ShowRefundAmount(_unit.UnitData.baseUnitCost);
    }

    private void ClearDiscardUnitDropZoneHighlight()
    {
        if (_hoveredDiscardDropZone == null) return;

        _hoveredDiscardDropZone.HideDiscardDropZoneHighlight();
        _hoveredDiscardDropZone.RevertRefundAmount();
        _hoveredDiscardDropZone = null;
    }

    private void CleanupAfterDrag()
    {
        CleanupDragSprite();
        ClearHighlightedTileNode();
        ClearInventoryDropZoneHighlight();
        ClearDiscardUnitDropZoneHighlight();
        RaiseToggleInventoryDropZoneEvent(false);
        RaiseToggleDiscardDropZoneEvent(false);
    }

    private void CleanupDragSprite()
    {
        if (_dragSprite != null)
        {
            _dragVisualPoolService.ReleaseDragSprite();
            _dragSprite = null;
        }
        _unit.UpdateUnitSpriteVisibility(true);
        _unit.UpdateUnitUIVisibility(true);
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
