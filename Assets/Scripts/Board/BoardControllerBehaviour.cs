using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class BoardControllerBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform boardContainer;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private CardView cardPrefab;

    private BoardController controller;
    private CardMatchResolverBehaviour resolver;
    private BoardSettings currentSettings;
    private EventBus bus;

    public void Initialize(BoardSettings settings, CardView cardPrefab)
    {
        this.cardPrefab = cardPrefab;
        Rebuild(settings);
    }

    public void Rebuild(BoardSettings settings)
    {
        currentSettings = settings;
        EnsureInitialized();
        controller.BuildBoard(settings.gridSize);
        resolver.Initialize(
            controller.Models,
            controller.Views,
            bus,
            settings.mismatchFlipBackDelay,
            settings.matchHideDelay
        );
        StopAllCoroutines();
        StartCoroutine(PreviewCards(settings.previewFaceUpDuration));
    }

    private IEnumerator PreviewCards(float duration)
    {
        // Disable input while showing the initial preview.
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

    private void EnsureInitialized()
    {
        if (controller == null)
            controller = new BoardController(boardContainer, grid, currentSettings, cardPrefab, bus);

        if (resolver == null)
            resolver = gameObject.AddComponent<CardMatchResolverBehaviour>();

        if (bus == null)
        {
            var gameController = Object.FindFirstObjectByType<GameController>();
            bus = gameController != null ? gameController.Bus : null;
        }
    }

    private void Awake()
    {
        if (boardContainer == null || grid == null || cardPrefab == null)
        {
            Debug.LogError("BoardControllerBehaviour: internal prefab references are missing.");
            enabled = false;
        }
    }
}
