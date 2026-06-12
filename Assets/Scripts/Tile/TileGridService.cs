using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridService
{
    private Tilemap _spawnedTileMap;
    private Grid _tileGrid;

    public void CreateTileMap(Tilemap _tileMapPrefab, Grid tileGrid)
    {
        _tileGrid = tileGrid;
        _spawnedTileMap = GameObject.Instantiate(_tileMapPrefab, _tileGrid.transform);
    }

    public void Reset()
    {
        if (_spawnedTileMap != null)
        {
            Object.Destroy(_spawnedTileMap.gameObject);
            _spawnedTileMap = null;
        }

        _tileGrid = null;
    }

    public Tilemap CurrentTileMap => _spawnedTileMap;
}
