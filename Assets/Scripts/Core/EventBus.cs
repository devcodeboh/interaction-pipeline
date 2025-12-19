using System;


public sealed  class EventBus
{
    public event Action<GameEvents.PhaseChanged> OnPhaseChanged;
    public event Action<GameEvents.GameCompleted> OnGameCompleted;
    public event Action<GameEvents.CardClicked> OnCardClicked;
    public event Action<GameEvents.CardFlipStarted> OnCardFlipStarted;

    public void Publish(GameEvents.PhaseChanged eventData) => OnPhaseChanged?.Invoke(eventData);
    public void Publish(GameEvents.GameCompleted eventData) => OnGameCompleted?.Invoke(eventData);
    public void Publish(GameEvents.CardClicked eventData) => OnCardClicked?.Invoke(eventData);
    public void Publish(GameEvents.CardFlipStarted eventData) => OnCardFlipStarted?.Invoke(eventData);
}
