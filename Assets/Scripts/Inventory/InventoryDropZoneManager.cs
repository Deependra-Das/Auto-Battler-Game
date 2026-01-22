using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZoneManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        BaseUnit baseUnit = eventData.pointerDrag?.GetComponent<BaseUnit>();
        if (baseUnit == null)
            return;

        BaseUnit unit = baseUnit.GetComponent<BaseUnit>();
        if (unit == null)
            return;

        if (unit.CurrentNode != null)
            unit.CurrentNode.SetOccupied(false);

        InventoryService inventory = GameManager.Instance.Get<InventoryService>();
        //inventory.AddUnit(baseUnit.UnitData);

        Destroy(unit.gameObject);
    }
}
