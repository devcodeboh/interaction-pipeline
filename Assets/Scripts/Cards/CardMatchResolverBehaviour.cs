using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CardMatchResolverBehaviour : MonoBehaviour
{
    private IReadOnlyList<CardModel> models;
    private IReadOnlyList<CardView> views;
    private EventBus bus;
    private float mismatchDelay;
    private float matchHideDelay;
    private bool isEnabled = true;
    private int generation;

    private readonly List<int> revealedOrder = new();
    private readonly HashSet<int> resolving = new();

    public void Initialize(IReadOnlyList<CardModel> models, IReadOnlyList<CardView> views, EventBus bus, float mismatchDelay, float matchHideDelay)
    {
        Unsubscribe();
        StopAllCoroutines();
        revealedOrder.Clear();
        resolving.Clear();
        generation++;

        this.models = models;
        this.views = views;
        this.bus = bus;
        this.mismatchDelay = Mathf.Max(0f, mismatchDelay);
        this.matchHideDelay = Mathf.Max(0f, matchHideDelay);

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

        if (!revealedOrder.Contains(eventData.Index))
            revealedOrder.Add(eventData.Index);

        TryResolvePairs();
    }

    private void TryResolvePairs()
    {
        // Compare the most recently revealed pair to keep input continuous.
        while (TryGetLastPair(out int first, out int second))
        {
            resolving.Add(first);
            resolving.Add(second);

            var firstModel = models[first];
            var secondModel = models[second];

            if (firstModel.PairId == secondModel.PairId)
            {
                firstModel.SetState(CardState.Matched);
                secondModel.SetState(CardState.Matched);
                StartCoroutine(HideMatchedAfterDelay(first, second, generation));

                revealedOrder.Remove(first);
                revealedOrder.Remove(second);

                bus?.Publish(new GameEvents.CardMatchResolved(first, second, firstModel.PairId));
                continue;
            }

            StartCoroutine(FlipBackAfterDelay(first, second, generation));
            revealedOrder.Remove(first);
            revealedOrder.Remove(second);
        }
    }

    private IEnumerator FlipBackAfterDelay(int first, int second, int token)
    {
        if (mismatchDelay > 0f)
            yield return new WaitForSeconds(mismatchDelay);

        if (token != generation)
            // Board was rebuilt; ignore stale coroutine.
            yield break;

        FlipDownIfNeeded(first);
        FlipDownIfNeeded(second);

        resolving.Remove(first);
        resolving.Remove(second);

        bus?.Publish(new GameEvents.CardMismatchResolved(first, second));

        TryResolvePairs();
    }

    private IEnumerator HideMatchedAfterDelay(int first, int second, int token)
    {
        if (matchHideDelay > 0f)
            yield return new WaitForSeconds(matchHideDelay);

        if (token != generation)
            // Board was rebuilt; ignore stale coroutine.
            yield break;

        HideIfValid(first);
        HideIfValid(second);
    }

    private void HideIfValid(int index)
    {
        if (index < 0 || index >= views.Count)
            return;

        var view = views[index];
        if (view == null)
            return;

        view.SetMatchedHidden(true);
        resolving.Remove(index);
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

    private bool TryGetLastPair(out int first, out int second)
    {
        first = -1;
        second = -1;

        for (int i = revealedOrder.Count - 1; i >= 0; i--)
        {
            int index = revealedOrder[i];
            if (!IsCandidate(index))
                continue;

            if (second == -1)
            {
                second = index;
                continue;
            }

            first = index;
            break;
        }

        return first != -1 && second != -1;
    }

    private bool IsCandidate(int index)
    {
        if (index < 0 || index >= models.Count)
            return false;

        if (resolving.Contains(index))
            return false;

        return models[index].State == CardState.FaceUp;
    }

    private void Unsubscribe()
    {
        if (bus != null)
            bus.OnCardFlipStarted -= HandleCardFlipStarted;
    }
}
