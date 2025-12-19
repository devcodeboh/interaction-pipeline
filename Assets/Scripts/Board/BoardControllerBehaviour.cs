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

    public bool IsInitialized => controller != null;

    public void Initialize(BoardSettings settings, CardView cardPrefab)
    {
        if (cardPrefab != null)
            this.cardPrefab = cardPrefab;

        currentSettings = settings;
        Rebuild(settings);
    }

    public void Rebuild(BoardSettings settings)
    {
        currentSettings = settings;
        if (!EnsureInitialized())
            return;

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

    private bool EnsureInitialized()
    {
        if (bus == null)
        {
            var gameController = Object.FindFirstObjectByType<GameController>();
            bus = gameController != null ? gameController.Bus : null;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("BoardControllerBehaviour: Card Prefab is missing.");
            return false;
        }

        if (controller == null)
            controller = new BoardController(boardContainer, grid, currentSettings, cardPrefab, bus);

        if (resolver == null)
            resolver = gameObject.AddComponent<CardMatchResolverBehaviour>();

        return true;
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
