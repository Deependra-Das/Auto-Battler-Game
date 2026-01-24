using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZoneManager : MonoBehaviour, IDropHandler
{
    InventoryService inventoryServiceObj;
    TeamService teamServiceObj;

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        unit.MarkDroppedOnValidZone();

        if (unit.CurrentNode != null)
            unit.CurrentNode.SetOccupied(false);

        inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        inventoryServiceObj.AddUnit(unit.UnitData);

        teamServiceObj = GameManager.Instance.Get<TeamService>();
        teamServiceObj.RemoveUnitFromTeam(unit, unit.Team);
        Destroy(unit.gameObject);

    }
}
