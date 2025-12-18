using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameController : MonoBehaviour
{
    public GamePhase GetPhase => State.Phase;
    public GameState State { get; } = new GameState();
    private void Awake() => SetPhase(GamePhase.Boot);
    private void Start() => SetPhase(GamePhase.Playing);
    private void SetPhase(GamePhase newPhase) => State.SetPhase(newPhase);
}
