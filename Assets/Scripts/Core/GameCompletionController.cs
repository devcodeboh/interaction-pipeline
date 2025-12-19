using UnityEngine;

public sealed class GameCompletionController : MonoBehaviour
{
    private BoardControllerBehaviour board;
    private GameController gameController;
    private EventBus bus;
    private bool completed;

    public void Initialize(BoardControllerBehaviour board, GameController gameController, EventBus bus)
    {
        this.board = board;
        this.gameController = gameController;
        this.bus = bus;

        if (this.bus != null)
            this.bus.OnCardMatchResolved += HandleMatchResolved;
    }

    public void ResetCompletion()
    {
        completed = false;
    }

    public void CheckNow()
    {
        HandleMatchResolved(default);
    }

    private void OnDestroy()
    {
        if (bus != null)
            bus.OnCardMatchResolved -= HandleMatchResolved;
    }

    private void HandleMatchResolved(GameEvents.CardMatchResolved _)
    {
        if (completed || board == null)
            return;

        int totalCards = board.CurrentGridSize.x * board.CurrentGridSize.y;
        if (totalCards <= 0)
            return;

        if (board.GetMatchedCount() < totalCards)
            return;

        completed = true;
        board.SetInputEnabled(false);
        gameController?.CompleteGame();
    }
}
