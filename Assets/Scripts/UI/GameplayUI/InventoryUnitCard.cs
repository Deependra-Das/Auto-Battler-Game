using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUnitCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _unitFaction;
    [SerializeField] private Image _unitType;
    [SerializeField] private Image _unitLevel;

    public UnitData UnitData { get; private set; }

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private LayoutElement _layoutElement;

    private Transform _cardContainer;
    private GameObject _placeholder;

    private void Awake()
    {
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        _cardContainer = transform.parent;
    }

    public void Initialize(UnitData unitData, Canvas canvas)
    {
        UnitData = unitData;
        _unitIcon.sprite = unitData.unitIcon;
        _canvas = canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _placeholder = new GameObject("Placeholder");
        _placeholder.transform.SetParent(_cardContainer);
        _placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        LayoutElement placeholderLayout = _placeholder.AddComponent<LayoutElement>();
        placeholderLayout.preferredWidth = _layoutElement.preferredWidth;
        placeholderLayout.preferredHeight = _layoutElement.preferredHeight;

        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.85f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        int targetIndex = _cardContainer.childCount;

        for (int i = 0; i < _cardContainer.childCount; i++)
        {
            if (transform.position.x < _cardContainer.GetChild(i).position.x)
            {
                targetIndex = i;
                break;
            }
        }

        _placeholder.transform.SetSiblingIndex(targetIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_cardContainer);
        transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        Destroy(_placeholder);

        UIManager.Instance.RefreshInventoryOrder();
    }
}
