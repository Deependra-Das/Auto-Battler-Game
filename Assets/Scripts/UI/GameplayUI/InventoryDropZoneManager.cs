using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDropZoneManager : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _highlightInventoryPanel;
    [SerializeField] private Color _validColor;
    [SerializeField] private Color _wrongColor;

    private InventoryService _inventoryServiceObj;
    private TeamService _teamServiceObj;
    private bool _isInventoryHighlightActive = false;

    public static InventoryDropZoneManager Instance;

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_highlightInventoryPanel != null)
            _highlightInventoryPanel.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.HighlightInventoryPanel, OnInteractInventorySetHighlight);
    }
    private void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.HighlightInventoryPanel, OnInteractInventorySetHighlight);
    }

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        if (!_inventoryServiceObj.CanAddUnit) return;

        UnitDragHandler dragHandler = eventData.pointerDrag.GetComponent<UnitDragHandler>();
        if (dragHandler != null)
            dragHandler.MarkDroppedOnInventoryZone();

        _teamServiceObj.MoveToInventory(unit, unit.Team);
        _inventoryServiceObj.AddUnit(unit.UnitData);
        Destroy(unit.gameObject);
    }

    private void OnInteractInventorySetHighlight(object[] parameters)
    {
        _isInventoryHighlightActive = (bool)parameters[0];

        if (!_isInventoryHighlightActive)
        {
            _highlightInventoryPanel.gameObject.SetActive(false);
            return;
        }

        bool canAdd = _inventoryServiceObj.CanAddUnit;

        _highlightInventoryPanel.gameObject.SetActive(true);
        _highlightInventoryPanel.color = canAdd ? _validColor : _wrongColor;
    }
}
