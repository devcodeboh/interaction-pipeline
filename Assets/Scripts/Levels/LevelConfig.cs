using UnityEngine;

[CreateAssetMenu(menuName = "Config/Level Config", fileName = "LevelConfig")]
public sealed class LevelConfig : ScriptableObject
{
    public LevelPreset easy;
    public LevelPreset medium;
    public LevelPreset hard;

    public LevelPreset GetPreset(LevelDifficulty difficulty)
    {
        return difficulty switch
        {
            LevelDifficulty.Easy => easy,
            LevelDifficulty.Medium => medium,
            LevelDifficulty.Hard => hard,
            _ => easy
        };
    }
}
