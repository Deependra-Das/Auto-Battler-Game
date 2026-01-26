using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZoneManager : MonoBehaviour, IDropHandler
{
    private SpriteRenderer _highlightPanel;

    private InventoryService _inventoryServiceObj;
    private TeamService _teamServiceObj;

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        unit.MarkDroppedOnValidZone();

        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _inventoryServiceObj.AddUnit(unit.UnitData);

        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _teamServiceObj.RemoveUnitFromTeam(unit, unit.Team);
        Destroy(unit.gameObject);

    }

    public void SetHighlight(bool active)
    {
        _highlightPanel.gameObject.SetActive(active);
    }
}
