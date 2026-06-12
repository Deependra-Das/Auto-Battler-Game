using UnityEngine;

public class HighlightTileService
{
    private GameObject _highlightTilePrefab;

    private GameObject _highlightTile;
    private SpriteRenderer _renderer;

    private Sprite _validHighlightSprite;
    private Sprite _invalidHighlightSprite;

    public HighlightTileService(TileScriptableObjectScript tile_SO)
    {
        _highlightTilePrefab = tile_SO.highlightTilePrefab;
        _validHighlightSprite = tile_SO.validHighlightSprite;
        _invalidHighlightSprite = tile_SO.invalidHighlightSprite;
    }

    public void Initialize()
    {
        _highlightTile = Object.Instantiate(_highlightTilePrefab);
        _highlightTile.SetActive(false);

        _renderer = _highlightTile.GetComponent<SpriteRenderer>();
    }

    public void ShowTileHighlight(Vector3 worldPosition, bool valid)
    {
        _highlightTile.transform.position = worldPosition;

        _renderer.sprite = valid ? _validHighlightSprite : _invalidHighlightSprite;

        if (!_highlightTile.activeSelf)
            _highlightTile.SetActive(true);
    }

    public void HideTileHighlight()
    {
        if (_highlightTile.activeSelf)
            _highlightTile.SetActive(false);
    }

    public void Dispose()
    {
        if (_highlightTile != null)
        {
            _highlightTile.SetActive(false);
            Object.Destroy(_highlightTile);
        }
    }
}