using UnityEngine;

public sealed class GameSaveController : MonoBehaviour
{
    private readonly SaveService service = new SaveService();
    private BoardControllerBehaviour board;
    private GameSessionController session;
    private GameStatsController stats;
    private EventBus bus;
    private bool isRestoring;

    public void Initialize(BoardControllerBehaviour board, GameSessionController session, GameStatsController stats, EventBus bus)
    {
        this.board = board;
        this.session = session;
        this.stats = stats;
        this.bus = bus;

        if (this.bus != null)
        {
            this.bus.OnCardMatchResolved += HandleProgressChanged;
            this.bus.OnCardMismatchResolved += HandleProgressChanged;
        }

        TryLoad();
    }

    private void OnDestroy()
    {
        if (bus == null)
            return;

        bus.OnCardMatchResolved -= HandleProgressChanged;
        bus.OnCardMismatchResolved -= HandleProgressChanged;
    }

    private void HandleProgressChanged(GameEvents.CardMatchResolved _) => SaveNow();
    private void HandleProgressChanged(GameEvents.CardMismatchResolved _) => SaveNow();

    private void TryLoad()
    {
        var data = service.Load();
        if (data == null)
            return;

        isRestoring = true;
        session.LoadFromSave(data);
        isRestoring = false;
    }

    private void SaveNow()
    {
        if (isRestoring || board == null || session == null || stats == null)
            return;

        if (!board.IsInitialized)
            return;

        var pairIds = board.GetPairIds();
        var states = board.GetCardStates();
        if (pairIds == null || states == null || pairIds.Length != states.Length)
            return;

        var grid = board.CurrentGridSize;
        var data = new GameSaveData
        {
            difficulty = (int)session.CurrentDifficulty,
            gridX = grid.x,
            gridY = grid.y,
            pairIds = pairIds,
            cardStates = states,
            matches = stats.GetMatches(),
            turns = stats.GetTurns()
        };

        service.Save(data);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveNow();
    }

    private void OnApplicationQuit()
    {
        SaveNow();
    }
}
