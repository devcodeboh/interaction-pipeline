using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CardMatchResolverBehaviour : MonoBehaviour
{
    private IReadOnlyList<CardModel> models;
    private IReadOnlyList<CardView> views;
    private EventBus bus;
    private float mismatchDelay;
    private bool isEnabled = true;

    private readonly List<int> faceUp = new();
    private readonly HashSet<int> resolving = new();

    public void Initialize(IReadOnlyList<CardModel> models, IReadOnlyList<CardView> views, EventBus bus, float mismatchDelay)
    {
        Unsubscribe();

        this.models = models;
        this.views = views;
        this.bus = bus;
        this.mismatchDelay = Mathf.Max(0f, mismatchDelay);

        if (this.bus != null)
            this.bus.OnCardFlipStarted += HandleCardFlipStarted;
    }

    public void SetEnabled(bool enabled) => isEnabled = enabled;

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void HandleCardFlipStarted(GameEvents.CardFlipStarted eventData)
    {
        if (!isEnabled)
            return;

        if (!eventData.FaceUp)
            return;

        if (models == null || views == null)
            return;

        if (eventData.Index < 0 || eventData.Index >= models.Count)
            return;

        var model = models[eventData.Index];
        if (model == null || model.State != CardState.FaceUp)
            return;

        if (!faceUp.Contains(eventData.Index))
            faceUp.Add(eventData.Index);

        TryResolvePairs();
    }

    private void TryResolvePairs()
    {
        while (TryGetNextPair(out int first, out int second))
        {
            resolving.Add(first);
            resolving.Add(second);

            var firstModel = models[first];
            var secondModel = models[second];

            if (firstModel.PairId == secondModel.PairId)
            {
                firstModel.SetState(CardState.Matched);
                secondModel.SetState(CardState.Matched);
                views[first].SetMatchedHidden(true);
                views[second].SetMatchedHidden(true);

                faceUp.Remove(first);
                faceUp.Remove(second);
                resolving.Remove(first);
                resolving.Remove(second);

                bus?.Publish(new GameEvents.CardMatchResolved(first, second, firstModel.PairId));
                continue;
            }

            StartCoroutine(FlipBackAfterDelay(first, second));
        }
    }

    private IEnumerator FlipBackAfterDelay(int first, int second)
    {
        if (mismatchDelay > 0f)
            yield return new WaitForSeconds(mismatchDelay);

        FlipDownIfNeeded(first);
        FlipDownIfNeeded(second);

        faceUp.Remove(first);
        faceUp.Remove(second);
        resolving.Remove(first);
        resolving.Remove(second);

        bus?.Publish(new GameEvents.CardMismatchResolved(first, second));

        TryResolvePairs();
    }

    private void FlipDownIfNeeded(int index)
    {
        if (index < 0 || index >= models.Count || index >= views.Count)
            return;

        var model = models[index];
        var view = views[index];
        if (model == null || view == null)
            return;

        if (model.State != CardState.FaceUp)
            return;

        model.SetState(CardState.FaceDown);
        bus?.Publish(new GameEvents.CardFlipStarted(index, false));
        view.PlayFlip(false);
    }

    private bool TryGetNextPair(out int first, out int second)
    {
        first = -1;
        second = -1;

        for (int i = 0; i < faceUp.Count; i++)
        {
            int index = faceUp[i];
            if (index < 0 || index >= models.Count)
                continue;

            if (resolving.Contains(index))
                continue;

            if (models[index].State != CardState.FaceUp)
                continue;

            first = index;
            break;
        }

        if (first == -1)
            return false;

        for (int i = 0; i < faceUp.Count; i++)
        {
            int index = faceUp[i];
            if (index == first)
                continue;

            if (index < 0 || index >= models.Count)
                continue;

            if (resolving.Contains(index))
                continue;

            if (models[index].State != CardState.FaceUp)
                continue;

            second = index;
            break;
        }

        return second != -1;
    }

    private void Unsubscribe()
    {
        if (bus != null)
            bus.OnCardFlipStarted -= HandleCardFlipStarted;
    }
}
