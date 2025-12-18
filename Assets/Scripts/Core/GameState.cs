public sealed class GameState
{
    public GamePhase Phase { get; private set; } = GamePhase.None;

    internal void SetPhase(GamePhase newPhase) => Phase = newPhase;
    // placeholders for future game state properties:
    // public int Score;
    // public int MatchedCount;
    // public int TotalCards;
}
