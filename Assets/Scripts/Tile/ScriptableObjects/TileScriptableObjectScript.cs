using UnityEngine;

[CreateAssetMenu(fileName = "TileScriptableObjectScript", menuName = "ScriptableObjects/TileScriptableObjectScript")]
public class TileScriptableObjectScript : ScriptableObject
{
    public GameObject highlightTilePrefab;
    public Sprite validHighlightSprite;
    public Sprite invalidHighlightSprite;
}
