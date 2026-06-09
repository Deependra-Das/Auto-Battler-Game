using AutoBattler.Main;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDropZoneManager : MonoBehaviour
{
    [SerializeField] private Image _highlightValidInventoryImage;
    [SerializeField] private Image _highlightInvalidInventoryImage;

    private InventoryService _inventoryServiceObj;
    private TeamService _teamServiceObj;
    private UnitPoolService _unitPoolServiceObj;

    protected void Awake()
    {
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _unitPoolServiceObj = GameManager.Instance.Get<UnitPoolService>();

        HideInventoryHighlight();
    }

    public void HandleUnitDrop(BaseUnit baseUnit)
    {
        if (!_inventoryServiceObj.CanAddUnit) return;

        _teamServiceObj.MoveToInventory(baseUnit, baseUnit.Team);
        _inventoryServiceObj.AddUnit(baseUnit.UnitData);
        _unitPoolServiceObj.Release(baseUnit.UnitData.unitID, baseUnit);
    }

    private void ToggleValidHighlight(bool state)
    {
        if (_highlightValidInventoryImage != null)
            _highlightValidInventoryImage.enabled = state;
    }
    private void ToggleInvalidHighlight(bool state)
    {
        if (_highlightInvalidInventoryImage != null)
            _highlightInvalidInventoryImage.enabled = state;
    }

    public void ShowInventoryHighlight()
    {
        switch(CheckValid())
        {
            case true:
                ToggleValidHighlight(true);
                ToggleInvalidHighlight(false);
                break;

            case false:
                ToggleValidHighlight(false);
                ToggleInvalidHighlight(true);
                break;
        }
    }

    public void HideInventoryHighlight()
    {
        ToggleValidHighlight(false);
        ToggleInvalidHighlight(false);
    }

    private bool CheckValid()
    {
        return _inventoryServiceObj.CanAddUnit;
    }
}
