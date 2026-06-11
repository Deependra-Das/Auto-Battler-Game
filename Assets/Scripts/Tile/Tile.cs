using UnityEngine;

public class Tile : MonoBehaviour
{
    public Node Node { get; set; }
    [SerializeField] private SpriteRenderer _highlightSprite;
    [SerializeField] private Color _validColor;
    [SerializeField] private Color _wrongColor;

    public void OnInteractShowHighlight()
    {
        SetHighlightColor();
        _highlightSprite.gameObject.SetActive(true);
    }

    public void HideHighlight()
    {
        _highlightSprite.gameObject.SetActive(false);
    }

    private void SetHighlightColor()
    {
        bool isValid = GameplayManager.Instance.CurrentGameplayState == GameplayStateEnum.Preparation &&
        Node != null && !Node.IsOccupied;

        _highlightSprite.color = isValid ? _validColor : _wrongColor;
    }
}