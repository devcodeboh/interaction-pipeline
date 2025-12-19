using System.Collections.Generic;

public sealed class CardInputController
{
    private readonly IReadOnlyList<CardModel> models;
    private readonly IReadOnlyList<CardView> views;

    public CardInputController(IReadOnlyList<CardModel> models, IReadOnlyList<CardView> views)
    {
        this.models = models;
        this.views = views;
    }

    public void HandleCardClicked(int index)
    {
        if (index < 0 || index >= models.Count || index >= views.Count)
            return;

        var model = models[index];
        var view = views[index];

        if (model == null || view == null)
            return;

        if (!model.CanFlip || view.IsAnimating)
            return;

        model.SetState(CardState.FaceUp);
        view.PlayFlip(true);
    }
}
