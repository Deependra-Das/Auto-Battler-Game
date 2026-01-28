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

    private void Awake()
    {
        _highlightInventoryPanel.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        Debug.Log(_inventoryServiceObj.CanAddUnit +" "+ _inventoryServiceObj.MaxInventorySize);
        if (!_inventoryServiceObj.CanAddUnit) return;

        UnitDragHandler dragHandler = eventData.pointerDrag.GetComponent<UnitDragHandler>();
        if (dragHandler != null)
            dragHandler.MarkDroppedOnValidZone();

        _inventoryServiceObj.AddUnit(unit.UnitData);

        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _teamServiceObj.RemoveUnitFromTeam(unit, unit.Team);
        Destroy(unit.gameObject);
    }

    public void OnInteractSetHighlight(bool active)
    {
        if (!active)
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
