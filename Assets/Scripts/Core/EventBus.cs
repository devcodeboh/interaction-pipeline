using System;


public sealed  class EventBus
{
    public event Action<GameEvents.PhaseChanged> OnPhaseChanged;
    public event Action<GameEvents.GameCompleted> OnGameCompleted;

    public void Publish(GameEvents.PhaseChanged eventData) => OnPhaseChanged?.Invoke(eventData);
    public void Publish(GameEvents.GameCompleted eventData) => OnGameCompleted?.Invoke(eventData);
}