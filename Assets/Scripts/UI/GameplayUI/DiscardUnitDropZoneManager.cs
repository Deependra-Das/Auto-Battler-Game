using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardUnitDropZoneManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _highlightDiscardImage;

    private TeamService _teamServiceObj;
    private InventoryService _inventoryServiceObj;
    private CurrencyService _currencyServiceObj;

    private void Awake()
    {
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        SetHighlight(false);
    }

    private void OnDestroy()
    {
        _teamServiceObj = null;
        _inventoryServiceObj = null;
        _currencyServiceObj = null;
    }

    public void HandleUnitDrop(GameObject dragged)
    {
        if (!dragged.TryGetComponent(out BaseUnit baseUnit))
            return;

        int refundAmount = baseUnit.UnitData.unitCost;

        if (dragged.TryGetComponent<UnitDragHandler>(out var dragHandler))
            dragHandler.MarkDroppedOnDiscardUnitZone();

        _teamServiceObj.RemoveUnitFromField(baseUnit, baseUnit.Team, true);
        _teamServiceObj.RemoveUnitFromTeam(baseUnit.UnitData, baseUnit.Team);

        HandleRefund(refundAmount);
        Destroy(baseUnit.gameObject);
    }

    public void HandleInventoryDrop(InventoryUnitCard inventoryUnitCard)
    {
        int refundAmount = inventoryUnitCard.UnitData.unitCost;
        _teamServiceObj.RemoveUnitFromInventory(inventoryUnitCard.UnitData, TeamEnum.Team1);
        _teamServiceObj.RemoveUnitFromTeam(inventoryUnitCard.UnitData, TeamEnum.Team1);
        _inventoryServiceObj.RemoveUnit(inventoryUnitCard);
        EventBusManager.Instance.Raise(EventNameEnum.InventoryUnitCardDiscarded);
        HandleRefund(refundAmount);
        SetHighlight(false);
    }

    private void HandleRefund(int refundAmount)
    {
        _currencyServiceObj.AddCurrency(refundAmount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);
    }

    private void SetHighlight(bool state)
    {
        if (_highlightDiscardImage != null)
            _highlightDiscardImage.enabled = state;
    }
}
