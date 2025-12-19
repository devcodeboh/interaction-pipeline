using UnityEngine;

public sealed class GameSessionController : MonoBehaviour
{
    private const LevelDifficulty DefaultDifficulty = LevelDifficulty.Easy;

    private BoardControllerBehaviour board;
    private BoardSettings settings;
    private LevelConfig levelConfig;
    private GameUIController ui;
    private CardView cardPrefab;
    private GameStatsController stats;

    private LevelDifficulty currentDifficulty = DefaultDifficulty;

    public LevelDifficulty CurrentDifficulty => currentDifficulty;

    public void Initialize(BoardControllerBehaviour board, BoardSettings settings, LevelConfig levelConfig, GameUIController ui, CardView cardPrefab, GameStatsController stats)
    {
        this.board = board;
        this.settings = settings;
        this.levelConfig = levelConfig;
        this.ui = ui;
        this.cardPrefab = cardPrefab;
        this.stats = stats;

        ui.DifficultySelected += HandleDifficultySelected;
        ui.PlayRequested += HandlePlayRequested;
        ui.HomeRequested += HandleHomeRequested;
        ui.NextRequested += HandleNextRequested;

        if (this.stats != null)
            this.stats.ResetStats();
    }

    public void LoadFromSave(GameSaveData data)
    {
        if (data == null || settings == null || board == null)
            return;

        currentDifficulty = (LevelDifficulty)data.difficulty;
        ApplyPreset(levelConfig.GetPreset(currentDifficulty), false);
        board.RestoreFromSave(
            settings,
            cardPrefab,
            new Vector2Int(data.gridX, data.gridY),
            data.pairIds,
            data.cardStates
        );
        stats?.SetStats(data.matches, data.turns);
        ui.ShowHud();
    }

    public void ShowMenu()
    {
        if (ui != null)
            ui.ShowMenu();
    }

    private void HandleDifficultySelected(LevelDifficulty difficulty)
    {
        currentDifficulty = difficulty;
    }

    private void HandlePlayRequested()
    {
        stats?.ResetStats();
        ApplyPreset(levelConfig.GetPreset(currentDifficulty), true);
        ui.ShowHud();
    }

    private void HandleHomeRequested()
    {
        ui.ShowMenu();
    }

    private void HandleNextRequested()
    {
        currentDifficulty = GetNextDifficulty(currentDifficulty);
        stats?.ResetStats();
        ApplyPreset(levelConfig.GetPreset(currentDifficulty), true);
    }

    private void ApplyPreset(LevelPreset preset, bool rebuildBoard)
    {
        if (preset == null || settings == null || board == null)
            return;

        // Apply preset to shared runtime settings.
        settings.gridSize = preset.gridSize;
        settings.previewFaceUpDuration = preset.previewFaceUpDuration;
        settings.mismatchFlipBackDelay = preset.mismatchFlipBackDelay;
        settings.matchHideDelay = preset.matchHideDelay;

        if (!rebuildBoard)
            return;

        if (!board.IsInitialized)
            board.Initialize(settings, cardPrefab);
        else
            board.Rebuild(settings);
    }

    private static LevelDifficulty GetNextDifficulty(LevelDifficulty current)
    {
        return current switch
        {
            LevelDifficulty.Easy => LevelDifficulty.Medium,
            LevelDifficulty.Medium => LevelDifficulty.Hard,
            _ => LevelDifficulty.Hard
        };
    }
}
