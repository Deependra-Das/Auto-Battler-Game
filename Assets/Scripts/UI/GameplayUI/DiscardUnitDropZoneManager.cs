using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardUnitDropZoneManager : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _highlightDiscardUnitPanel;
    [SerializeField] private Color _highlightColor;

    private void Awake()
    {
        _highlightDiscardUnitPanel.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var gameObject = eventData.pointerDrag.gameObject;
        if (!gameObject) return;

        int refundAmount = 0;
        if (gameObject.TryGetComponent<BaseUnit>(out BaseUnit baseUnit))
        {
            refundAmount = baseUnit.UnitData.unitLevel;
            Destroy(baseUnit.gameObject);
        }
        else if (gameObject.TryGetComponent<InventoryUnitCard>(out InventoryUnitCard inventoryUnitCard))
        {
            refundAmount = inventoryUnitCard.UnitData.unitLevel;
            Destroy(inventoryUnitCard.gameObject);
        }

        GameManager.Instance.Get<CurrencyService>().AddCurrency(refundAmount);
    }

    public void OnInteractSetHighlight(bool active)
    {
        if (!active)
        {
            _highlightDiscardUnitPanel.gameObject.SetActive(false);
            return;
        }

        _highlightDiscardUnitPanel.gameObject.SetActive(true);
        _highlightDiscardUnitPanel.color = _highlightColor;
    }
}
