using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;

    [SerializeField] private float _tileOffset_X;
    [SerializeField] private float _tileOffset_Y;

    void Start()
    {
        CreateTileMap();
    }

    void CreateTileMap()
    {
        float totalWidth = _mapWidth * _tileOffset_X;
        float totalHeight = _mapHeight * _tileOffset_Y;

        Vector3 centerPosition = new Vector3(-totalWidth / 2 + _tileOffset_X / 2, -totalHeight / 2, 0);

        for (int y = 0; y < _mapHeight; y++)
        {
            int tilesInRow = (y % 2 == 0) ? _mapWidth : _mapWidth - 1;

            for (int x = 0; x < tilesInRow; x++)
            {
                GameObject tile = Instantiate(_tilePrefab);

                float offsetX = (y % 2 == 0) ? x * _tileOffset_X : x * _tileOffset_X + _tileOffset_X / 2;
                tile.transform.position = centerPosition + new Vector3(offsetX, y * _tileOffset_Y, 0);
            }
        }
    }
}
