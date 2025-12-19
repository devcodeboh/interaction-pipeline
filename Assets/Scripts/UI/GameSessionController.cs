using UnityEngine;

public sealed class GameSessionController : MonoBehaviour
{
    private const LevelDifficulty DefaultDifficulty = LevelDifficulty.Easy;

    private BoardControllerBehaviour board;
    private BoardSettings settings;
    private LevelConfig levelConfig;
    private GameUIController ui;
    private CardView cardPrefab;

    private LevelDifficulty currentDifficulty = DefaultDifficulty;

    public void Initialize(BoardControllerBehaviour board, BoardSettings settings, LevelConfig levelConfig, GameUIController ui, CardView cardPrefab)
    {
        this.board = board;
        this.settings = settings;
        this.levelConfig = levelConfig;
        this.ui = ui;
        this.cardPrefab = cardPrefab;

        ui.DifficultySelected += HandleDifficultySelected;
        ui.PlayRequested += HandlePlayRequested;
        ui.HomeRequested += HandleHomeRequested;
        ui.NextRequested += HandleNextRequested;
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
        ApplyPreset(levelConfig.GetPreset(currentDifficulty));
        ui.ShowHud();
    }

    private void HandleHomeRequested()
    {
        ui.ShowMenu();
    }

    private void HandleNextRequested()
    {
        currentDifficulty = GetNextDifficulty(currentDifficulty);
        ApplyPreset(levelConfig.GetPreset(currentDifficulty));
    }

    private void ApplyPreset(LevelPreset preset)
    {
        if (preset == null || settings == null || board == null)
            return;

        // Apply preset to shared runtime settings.
        settings.gridSize = preset.gridSize;
        settings.previewFaceUpDuration = preset.previewFaceUpDuration;
        settings.mismatchFlipBackDelay = preset.mismatchFlipBackDelay;
        settings.matchHideDelay = preset.matchHideDelay;

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
