using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class BoardControllerBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform boardContainer;
    [SerializeField] private GridLayoutGroup grid;

    private BoardController controller;
    private CardMatchResolverBehaviour resolver;

    public void Initialize(BoardSettings settings, CardView cardPrefab)
    {
        var gameController = Object.FindFirstObjectByType<GameController>();
        var bus = gameController != null ? gameController.Bus : null;
        controller = new BoardController(boardContainer, grid, settings, cardPrefab, bus);
        controller.BuildBoard(settings.gridSize);
        resolver = gameObject.AddComponent<CardMatchResolverBehaviour>();
        resolver.Initialize(
            controller.Models,
            controller.Views,
            bus,
            settings.mismatchFlipBackDelay,
            settings.matchHideDelay
        );
        StartCoroutine(PreviewCards(settings.previewFaceUpDuration));
    }

    private IEnumerator PreviewCards(float duration)
    {
        controller.SetInputEnabled(false);
        if (resolver != null)
            resolver.SetEnabled(false);

        foreach (var model in controller.Models)
        {
            if (model.State == CardState.Matched)
                continue;

            model.SetState(CardState.FaceUp);
        }

        foreach (var view in controller.Views)
            view.PlayFlip(true);

        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        foreach (var model in controller.Models)
        {
            if (model.State == CardState.Matched)
                continue;

            model.SetState(CardState.FaceDown);
        }

        foreach (var view in controller.Views)
            view.PlayFlip(false);

        if (resolver != null)
            resolver.SetEnabled(true);

        controller.SetInputEnabled(true);
    }

    private void Awake()
    {
        if (boardContainer == null || grid == null)
        {
            Debug.LogError("BoardControllerBehaviour: internal prefab references are missing.");
            enabled = false;
        }
    }
}
