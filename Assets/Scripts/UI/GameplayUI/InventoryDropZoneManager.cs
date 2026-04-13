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

    private void Awake()
    {
        _highlightInventoryPanel.gameObject.SetActive(false);
    }
    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.HighlightInventoryPanel, OnInteractInventorySetHighlight);
    }
    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.HighlightInventoryPanel, OnInteractInventorySetHighlight);
    }

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        if (!_inventoryServiceObj.CanAddUnit) return;

        UnitDragHandler dragHandler = eventData.pointerDrag.GetComponent<UnitDragHandler>();
        if (dragHandler != null)
            dragHandler.MarkDroppedOnInventoryZone();

        _inventoryServiceObj.AddUnit(unit.UnitData);

        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _teamServiceObj.MoveToInventory(unit, unit.Team);
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

        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        bool canAdd = _inventoryServiceObj.CanAddUnit;

        _highlightInventoryPanel.gameObject.SetActive(true);
        _highlightInventoryPanel.color = canAdd ? _validColor : _wrongColor;
    }
}
