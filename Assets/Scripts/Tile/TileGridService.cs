using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridService
{
    private Tilemap _spawnedGameplayTileMap;
    private Tilemap _spawnedDeploymentTilemap;

    public void CreateGameplayTileMap(Tilemap _gameplayTileMapPrefab, Transform tileGridTransform)
    {
        _spawnedGameplayTileMap = GameObject.Instantiate(_gameplayTileMapPrefab, tileGridTransform);
    }

    public void CreateDeploymentTileMap(Tilemap _deploymentTileMapPrefab, Transform tileGridTransform)
    {
        _spawnedDeploymentTilemap = GameObject.Instantiate(_deploymentTileMapPrefab, tileGridTransform);
    }

    public void Reset()
    {
        if (_spawnedGameplayTileMap != null)
        {
            Object.Destroy(_spawnedGameplayTileMap.gameObject);
            _spawnedGameplayTileMap = null;
        }

        if (_spawnedDeploymentTilemap != null)
        {
            Object.Destroy(_spawnedDeploymentTilemap.gameObject);
            _spawnedDeploymentTilemap = null;
        }
    }

    public Tilemap CurrentGameplayTileMap => _spawnedGameplayTileMap;
    public Tilemap CurrentDeploymentTilemap => _spawnedDeploymentTilemap;
}
