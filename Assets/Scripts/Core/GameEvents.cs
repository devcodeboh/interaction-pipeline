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

    public readonly struct CardMatchResolved
    {
        public readonly int FirstIndex;
        public readonly int SecondIndex;
        public readonly int PairId;
        public CardMatchResolved(int firstIndex, int secondIndex, int pairId)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            PairId = pairId;
        }
    }

    public readonly struct CardMismatchResolved
    {
        public readonly int FirstIndex;
        public readonly int SecondIndex;
        public CardMismatchResolved(int firstIndex, int secondIndex)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
        }
    }
}

