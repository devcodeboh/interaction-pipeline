using UnityEngine;

public sealed class WinPopupController : MonoBehaviour
{
    private WinPopupView popup;
    private GameUIController ui;
    private EventBus bus;

    public void Initialize(WinPopupView popup, GameUIController ui, EventBus bus)
    {
        this.popup = popup;
        this.ui = ui;
        this.bus = bus;

        if (this.bus != null)
            this.bus.OnGameCompleted += HandleGameCompleted;
    }

    private void OnDestroy()
    {
        if (bus != null)
            bus.OnGameCompleted -= HandleGameCompleted;
    }

    private void HandleGameCompleted(GameEvents.GameCompleted _)
    {
        ui?.ShowWinPopup();
    }
}
