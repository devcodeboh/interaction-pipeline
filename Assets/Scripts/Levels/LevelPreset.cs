using UnityEngine;

[CreateAssetMenu(menuName = "Config/Level Preset", fileName = "LevelPreset")]
public sealed class LevelPreset : ScriptableObject
{
    public LevelDifficulty difficulty;
    public Vector2Int gridSize = new Vector2Int(4, 4);
    public float previewFaceUpDuration = 1.5f;
    public float mismatchFlipBackDelay = 0.6f;
    public float matchHideDelay = 1.5f;
}
