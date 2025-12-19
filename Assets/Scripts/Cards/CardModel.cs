public sealed class CardModel
{
    public int Index { get; }
    public int PairId { get; }
    public CardState State { get; private set; }

    public CardModel(int index, int pairId, CardState state = CardState.FaceDown)
    {
        Index = index;
        PairId = pairId;
        State = state;
    }

    public bool CanFlip => State == CardState.FaceDown;

    public void SetState(CardState state) => State = state;
}
