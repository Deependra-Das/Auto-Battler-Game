using AutoBattler.Event;
using AutoBattler.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardUnitDropZoneManager : MonoBehaviour
{
    [SerializeField] private Image _highlightDiscardImage;
    [SerializeField] private TMP_Text _refundAmountText;

    private TeamService _teamServiceObj;
    private InventoryService _inventoryServiceObj;
    private CurrencyService _currencyServiceObj;
    private UnitPoolService _unitPoolServiceObj;

    private string _zeroAmount = "0";

    private void Awake()
    {
        _teamServiceObj = GameManager.Instance.Get<TeamService>();
        _inventoryServiceObj = GameManager.Instance.Get<InventoryService>();
        _currencyServiceObj = GameManager.Instance.Get<CurrencyService>();
        _unitPoolServiceObj = GameManager.Instance.Get<UnitPoolService>();
        ToggleHighlight(false);
        RevertRefundAmount();
    }

    private void OnDestroy()
    {
        _teamServiceObj = null;
        _inventoryServiceObj = null;
        _currencyServiceObj = null;
    }

    public void HandleUnitDrop(BaseUnit baseUnit)
    {
        int refundAmount = baseUnit.UnitData.baseUnitCost;

        _teamServiceObj.RemoveUnitFromField(baseUnit, baseUnit.Team, true);
        _teamServiceObj.RemoveUnitFromTeam(baseUnit.UnitData, baseUnit.Team);
        _unitPoolServiceObj.Release(baseUnit.UnitData.unitID, baseUnit);
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.CoinRefund);
        HandleRefund(refundAmount);
        ToggleHighlight(false);
    }

    public void HandleInventoryDrop(InventoryUnitCard inventoryUnitCard)
    {
        int refundAmount = inventoryUnitCard.UnitData.baseUnitCost;
        _teamServiceObj.RemoveUnitFromInventory(inventoryUnitCard.UnitData, TeamEnum.Team1);
        _teamServiceObj.RemoveUnitFromTeam(inventoryUnitCard.UnitData, TeamEnum.Team1);
        _inventoryServiceObj.RemoveUnit(inventoryUnitCard);
        EventBusManager.Instance.Raise(EventNameEnum.InventoryUnitCardDiscarded);
        AudioManager.Instance.PlaySoundEffectsAudio(AudioTypeEnum.CoinRefund);
        HandleRefund(refundAmount);
        ToggleHighlight(false);
    }

    private void HandleRefund(int refundAmount)
    {
        _currencyServiceObj.AddCurrency(refundAmount);
    }

    private void ToggleHighlight(bool state)
    {
        if (_highlightDiscardImage != null)
            _highlightDiscardImage.enabled = state;
    }

    public void ShowDiscardDropZoneHighlight()
    {
        ToggleHighlight(true);
    }

    public void HideDiscardDropZoneHighlight()
    {
        ToggleHighlight(false);
    }

    public void ShowRefundAmount(int refundAmount)
    {
        _refundAmountText.text = refundAmount.ToString();
    }

    public void RevertRefundAmount()
    {
        _refundAmountText.text = _zeroAmount;
    }
}
