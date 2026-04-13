using AutoBattler.Event;
using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardUnitDropZoneManager : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _highlightDiscardUnitPanel;
    [SerializeField] private Color _highlightColor;

    private TeamService _teamServiceObj;
    private bool _isDiscardHighlightActive = false;

    private void Awake()
    {
        _highlightDiscardUnitPanel.gameObject.SetActive(false);
    }

    private void OnEnable() => SubscribeToEvents();
    private void OnDisable() => UnsubscribeToEvents();

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.HighlightDiscardPanel, OnInteractDiscardPanelSetHighlight);
    }
    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.HighlightDiscardPanel, OnInteractDiscardPanelSetHighlight);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var gameObject = eventData.pointerDrag.gameObject;
        if (!gameObject) return;

        int refundAmount = 0;
        _teamServiceObj = GameManager.Instance.Get<TeamService>();

        if (gameObject.TryGetComponent<BaseUnit>(out BaseUnit baseUnit))
        {
            refundAmount = baseUnit.UnitData.unitCost;
            UnitDragHandler dragHandler = eventData.pointerDrag.GetComponent<UnitDragHandler>();
            if (dragHandler != null)
                dragHandler.MarkDroppedOnDiscardUnitZone();

            _teamServiceObj.RemoveUnitFromField(baseUnit, baseUnit.Team);
            Destroy(baseUnit.gameObject);
        }
        else if (gameObject.TryGetComponent<InventoryUnitCard>(out InventoryUnitCard inventoryUnitCard))
        {
            refundAmount = inventoryUnitCard.UnitData.unitCost;
            inventoryUnitCard.MarkDroppedOnDiscardUnitZone();

            UIManager.Instance.RemoveInventoryUnitCard(inventoryUnitCard);
            _teamServiceObj.RemoveUnitFromInventory(inventoryUnitCard.UnitData, TeamEnum.Team1);
            Destroy(inventoryUnitCard.gameObject);
        }

        GameManager.Instance.Get<CurrencyService>().AddCurrency(refundAmount);
    }

    private void OnInteractDiscardPanelSetHighlight(object[] parameters)
    {
        _isDiscardHighlightActive = (bool)parameters[0];

        if (!_isDiscardHighlightActive)
        {
            _highlightDiscardUnitPanel.gameObject.SetActive(false);
            return;
        }

        _highlightDiscardUnitPanel.gameObject.SetActive(true);
        _highlightDiscardUnitPanel.color = _highlightColor;
    }
}
