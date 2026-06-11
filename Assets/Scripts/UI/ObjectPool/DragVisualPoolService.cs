using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragVisualPoolService
{
    private RectTransform _dragVisualPoolContainerRectTransform;
    Canvas _canvas;
    private GameObject _placeholder;
    private LayoutElement _placeholderLayoutElement;
    private Image _dragSprite;
    private RectTransform _dragSpriteRectTransform;

    public void Initialize(Canvas canvas, RectTransform dragVisualPoolContainerRectTransform)
    {
        _canvas = canvas;
        _dragVisualPoolContainerRectTransform = dragVisualPoolContainerRectTransform;
        CreatePlaceholder();
        CreateDragSprite();
    }

    private void CreatePlaceholder()
    {
        _placeholder = new GameObject("Placeholder");
        _placeholderLayoutElement = _placeholder.AddComponent<LayoutElement>();
        _placeholder.transform.SetParent(_dragVisualPoolContainerRectTransform, false);
        _placeholder.SetActive(false);
    }

    private void CreateDragSprite()
    {
        GameObject dragSprite_GO = new GameObject("DragSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragSprite_GO.transform.SetParent(_dragVisualPoolContainerRectTransform, false);
        _dragSprite = dragSprite_GO.GetComponent<Image>();
        _dragSprite.raycastTarget = false;
        _dragSpriteRectTransform = _dragSprite.GetComponent<RectTransform>();
        dragSprite_GO.SetActive(false);
    }

    public GameObject GetPlaceholder(Transform parent, int siblingIndex, float width, float height)
    {
        _placeholder.transform.SetParent(parent, false);
        _placeholder.transform.SetSiblingIndex(siblingIndex);

        _placeholderLayoutElement.preferredWidth = width;
        _placeholderLayoutElement.preferredHeight = height;

        _placeholder.SetActive(true);

        return _placeholder;
    }

    public void ReleasePlaceholder()
    {
        _placeholder.transform.SetParent(_dragVisualPoolContainerRectTransform, false);
        _placeholder.transform.localPosition = Vector3.zero;
        _placeholder.SetActive(false);
    }

    public Image GetDragSprite(Sprite sprite)
    {
        _dragSprite.transform.SetParent(_canvas.transform);
        _dragSprite.transform.SetAsLastSibling();
        _dragSprite.sprite = sprite;
        _dragSprite.SetNativeSize();
        _dragSprite.gameObject.SetActive(true);

        return _dragSprite;
    }

    public void ReleaseDragSprite()
    {
        _dragSprite.sprite = null;
        _dragSprite.transform.SetParent(_dragVisualPoolContainerRectTransform, false);
        _dragSprite.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        if (_placeholder != null)
            Object.Destroy(_placeholder);

        if (_dragSprite != null)
            Object.Destroy(_dragSprite.gameObject);

        _placeholder = null;
        _dragSprite = null;
        _placeholderLayoutElement = null;
    }

    public RectTransform GetDragSpriteRectTransform()
    {
        return _dragSpriteRectTransform;
    }
}
