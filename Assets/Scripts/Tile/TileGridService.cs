using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridService
{
    private Tilemap _spawnedGameplayTileMap;

    public void CreateGameplayTileMap(Tilemap _tileMapPrefab, Transform tileGridTransform)
    {
        _spawnedGameplayTileMap = GameObject.Instantiate(_tileMapPrefab, tileGridTransform);
    }

    public void Reset()
    {
        if (_spawnedGameplayTileMap != null)
        {
            Object.Destroy(_spawnedGameplayTileMap.gameObject);
            _spawnedGameplayTileMap = null;
        }
    }

    public Tilemap CurrentTileMap => _spawnedGameplayTileMap;
}
