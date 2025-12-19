using UnityEngine;

public sealed class GameStatsController : MonoBehaviour
{
    private HudView hud;
    private EventBus bus;
    private int matches;
    private int turns;

    public void Initialize(HudView hud, EventBus bus)
    {
        this.hud = hud;
        this.bus = bus;

        if (this.bus != null)
        {
            this.bus.OnCardMatchResolved += HandleMatchResolved;
            this.bus.OnCardMismatchResolved += HandleMismatchResolved;
        }

        ResetStats();
    }

    public void ResetStats()
    {
        matches = 0;
        turns = 0;
        UpdateHud();
    }

    public void SetStats(int matches, int turns)
    {
        this.matches = Mathf.Max(0, matches);
        this.turns = Mathf.Max(0, turns);
        UpdateHud();
    }

    public int GetMatches() => matches;
    public int GetTurns() => turns;

    private void HandleMatchResolved(GameEvents.CardMatchResolved _)
    {
        matches += 1;
        turns += 1;
        UpdateHud();
    }

    private void HandleMismatchResolved(GameEvents.CardMismatchResolved _)
    {
        turns += 1;
        UpdateHud();
    }

    private void UpdateHud()
    {
        if (hud == null)
            return;

        hud.SetMatches(matches);
        hud.SetTurns(turns);
    }

    private void OnDestroy()
    {
        if (bus == null)
            return;

        bus.OnCardMatchResolved -= HandleMatchResolved;
        bus.OnCardMismatchResolved -= HandleMismatchResolved;
    }
}
