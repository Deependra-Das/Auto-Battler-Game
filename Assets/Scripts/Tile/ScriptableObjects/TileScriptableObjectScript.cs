using UnityEngine;

[CreateAssetMenu(fileName = "TileScriptableObjectScript", menuName = "ScriptableObjects/TileScriptableObjectScript")]
public class TileScriptableObjectScript : ScriptableObject
{
    public Tile tilePrefab;
    public int mapWidth;
    public int mapHeight;
    public float tileOffset_X;
    public float tileOffset_Y;
}
