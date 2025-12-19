using UnityEngine;

[CreateAssetMenu(menuName = "Config/Board Settings", fileName = "BoardSettings")]
public sealed class BoardSettings : ScriptableObject
{
    [Header("Layout")]
    public Vector2Int gridSize = new Vector2Int(4, 4);

    [Min(0f)]
    public float spacing = 12f;

    [Min(0f)]
    public float padding = 24f;

    [Header("Matching")]
    [Min(0f)]
    public float mismatchFlipBackDelay = 0.6f;
}
