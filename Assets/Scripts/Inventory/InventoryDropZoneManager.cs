using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZoneManager : MonoBehaviour, IDropHandler
{
    InventoryService inventory;

    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit unit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (unit == null) return;

        unit.MarkDroppedOnValidZone();

        if (unit.CurrentNode != null)
            unit.CurrentNode.SetOccupied(false);

        inventory = GameManager.Instance.Get<InventoryService>();
        inventory.AddUnit(unit.UnitData);

        Destroy(unit.gameObject);
    }
}
