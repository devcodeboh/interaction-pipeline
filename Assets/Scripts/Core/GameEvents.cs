public static class GameEvents
{
    public readonly struct PhaseChanged
    {
        public readonly GamePhase From;
        public readonly GamePhase To;
        public PhaseChanged(GamePhase from, GamePhase to)
        {
            From = from;
            To = to;
        }
    }
    public readonly struct GameCompleted
    {
        public readonly GamePhase From;
        public readonly GamePhase To;
        public GameCompleted(GamePhase from, GamePhase to)
        {
            From = from;
            To = to;
        }
    }

    public readonly struct CardClicked
    {
        public readonly int Index;
        public CardClicked(int index) => Index = index;
    }

    public readonly struct CardFlipStarted
    {
        public readonly int Index;
        public readonly bool FaceUp;
        public CardFlipStarted(int index, bool faceUp)
        {
            Index = index;
            FaceUp = faceUp;
        }
    }
}

