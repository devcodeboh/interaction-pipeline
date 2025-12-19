using System.Collections.Generic;

public sealed class CardInputController
{
    private readonly IReadOnlyList<CardModel> models;
    private readonly IReadOnlyList<CardView> views;
    private readonly EventBus bus;
    private bool inputEnabled = true;

    public CardInputController(IReadOnlyList<CardModel> models, IReadOnlyList<CardView> views, EventBus bus)
    {
        this.models = models;
        this.views = views;
        this.bus = bus;
    }

    public void SetInputEnabled(bool enabled) => inputEnabled = enabled;

    public void HandleCardClicked(int index)
    {
        if (!inputEnabled)
            return;

        if (index < 0 || index >= models.Count || index >= views.Count)
            return;

        var model = models[index];
        var view = views[index];

        if (model == null || view == null)
            return;

        bus?.Publish(new GameEvents.CardClicked(index));

        if (!model.CanFlip || view.IsAnimating)
            return;

        model.SetState(CardState.FaceUp);
        view.PlayFlip(true);
        bus?.Publish(new GameEvents.CardFlipStarted(index, true));
    }
}
