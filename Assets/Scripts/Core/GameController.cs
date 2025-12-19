using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameController : MonoBehaviour
{
    public GameState State { get; } = new GameState();
    public EventBus Bus { get; } = new EventBus();

    public GamePhase Phase => State.Phase;

    private void Awake() => SetPhase(GamePhase.Boot);

    private void Start() => SetPhase(GamePhase.Playing);

    private void SetPhase(GamePhase newPhase)
    {
        var prev = State.Phase;
        if (prev == newPhase) return;

        State.SetPhase(newPhase);
        Bus.Publish(new GameEvents.PhaseChanged(prev, newPhase));
    }

    public void CompleteGame()
    {
        SetPhase(GamePhase.Completed);
        Bus.Publish(new GameEvents.GameCompleted());
    }
}
