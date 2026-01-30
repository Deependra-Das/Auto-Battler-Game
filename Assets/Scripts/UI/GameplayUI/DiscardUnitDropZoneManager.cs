using AutoBattler.Main;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardUnitDropZoneManager : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _highlightInventoryPanel;
    [SerializeField] private Color _validColor;
    [SerializeField] private Color _wrongColor;

    private void Awake()
    {
        _highlightInventoryPanel.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {        
    }

    public void OnInteractSetHighlight(bool active)
    {
    }
}
