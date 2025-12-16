using System.Collections.Generic;
using UnityEngine;

public class TileGridService
{
    private Tile _tilePrefab;
    private int _mapWidth;
    private int _mapHeight;
    private float _tileOffset_X;
    private float _tileOffset_Y;

    private List<Tile> _spawnedTileList = new List<Tile>();

    public TileGridService(TileScriptableObjectScript tile_SO)
    {
        _tilePrefab = tile_SO.tilePrefab;
        _mapWidth = tile_SO.mapWidth;
        _mapHeight = tile_SO.mapHeight;
        _tileOffset_X = tile_SO.tileOffset_X;
        _tileOffset_Y = tile_SO.tileOffset_Y;

        CreateTileMap();
    }

    private void CreateTileMap()
    {
        float totalWidth = _mapWidth * _tileOffset_X;
        float totalHeight = _mapHeight * _tileOffset_Y;

        Vector3 centerPosition = new Vector3(-totalWidth / 2 + _tileOffset_X / 2, -totalHeight / 2, 0);

        for (int y = 0; y < _mapHeight; y++)
        {
            int tilesInRow = (y % 2 == 0) ? _mapWidth : _mapWidth - 1;

            for (int x = 0; x < tilesInRow; x++)
            {
                Tile tile = GameObject.Instantiate(_tilePrefab);

                float offsetX = (y % 2 == 0) ? x * _tileOffset_X : x * _tileOffset_X + _tileOffset_X / 2;
                tile.transform.position = centerPosition + new Vector3(offsetX, y * _tileOffset_Y, 0);

                _spawnedTileList.Add(tile);
            }
        }
    }

    public List<Tile> GetSpawnedTilesList()
    {
        if(_spawnedTileList != null) return _spawnedTileList;

        return null;
    }
}
