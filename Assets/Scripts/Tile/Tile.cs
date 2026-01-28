using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Node Node { get; set; }
    [SerializeField] private SpriteRenderer _highlightSprite;
    [SerializeField] private Color _validColor;
    [SerializeField] private Color _wrongColor;

    public void OnInteractSetHighlight(bool active, bool valid)
    {
        _highlightSprite.gameObject.SetActive(active);
        _highlightSprite.color = valid ? _validColor : _wrongColor;
    }
}