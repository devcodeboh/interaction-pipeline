using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BoardController
{
    private readonly RectTransform boardContainer;
    private readonly GridLayoutGroup grid;
    private readonly BoardSettings settings;
    private readonly CardView cardPrefab;
    private readonly EventBus bus;

    private readonly List<CardView> spawnedCards = new();
    private readonly List<CardModel> models = new();
    private CardInputController inputController;

    public event Action<int> CardClicked;
    public IReadOnlyList<CardView> Views => spawnedCards;
    public IReadOnlyList<CardModel> Models => models;

    public BoardController(RectTransform boardContainer, GridLayoutGroup grid, BoardSettings settings, CardView cardPrefab, EventBus bus)
    {
        this.boardContainer = boardContainer;
        this.grid = grid;
        this.settings = settings;
        this.cardPrefab = cardPrefab;
        this.bus = bus;
    }

    public void BuildBoard(Vector2Int gridSize)
    {
        ClearBoard();
        ConfigureGrid(gridSize);
        SpawnCards(gridSize);
        inputController = new CardInputController(models, spawnedCards, bus);
        CardClicked += inputController.HandleCardClicked;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputController?.SetInputEnabled(enabled);
    }

    private void ConfigureGrid(Vector2Int gridSize)
    {
        int columns = Mathf.Max(1, gridSize.x);
        int rows = Mathf.Max(1, gridSize.y);

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.spacing = Vector2.one * settings.spacing;
        grid.padding = CreatePadding(settings.padding);

        float availableWidth =
            boardContainer.rect.width - settings.padding * 2f - settings.spacing * (columns - 1);

        float availableHeight =
            boardContainer.rect.height - settings.padding * 2f - settings.spacing * (rows - 1);

        float cellSize = Mathf.Floor(Mathf.Min(availableWidth / columns, availableHeight / rows));
        grid.cellSize = new Vector2(cellSize, cellSize);
    }

    private void SpawnCards(Vector2Int gridSize)
    {
        int count = Mathf.Max(1, gridSize.x) * Mathf.Max(1, gridSize.y);
        var pairIds = BuildPairIds(count);
        for (int i = 0; i < count; i++)
        {
            var card = UnityEngine.Object.Instantiate(cardPrefab, grid.transform);
            card.Bind(i);
            card.Clicked += HandleCardClicked;
            card.SetInstant(false);
            spawnedCards.Add(card);
            int pairId = pairIds[i];
            var model = new CardModel(i, pairId);
            if (pairId < 0)
                model.SetState(CardState.Matched);
            models.Add(model);
            card.SetBackSprite(settings.backSprite);
            card.SetFaceSprite(GetFaceSprite(pairId));
            if (pairId < 0)
                card.SetMatchedHidden(true);
        }
    }

    private void ClearBoard()
    {
        if (inputController != null)
            CardClicked -= inputController.HandleCardClicked;

        foreach (var card in spawnedCards)
        {
            if (card == null)
                continue;

            card.Clicked -= HandleCardClicked;
            UnityEngine.Object.Destroy(card.gameObject);
        }

        spawnedCards.Clear();
        models.Clear();
        inputController = null;
    }

    private static RectOffset CreatePadding(float value)
    {
        int v = Mathf.RoundToInt(value);
        return new RectOffset(v, v, v, v);
    }

    private Sprite GetFaceSprite(int pairId)
    {
        if (pairId < 0)
            return null;

        var sprites = settings.faceSprites;
        if (sprites == null || sprites.Length == 0)
            return null;

        if (pairId < 0 || pairId >= sprites.Length)
            return null;

        return sprites[pairId];
    }

    private static List<int> BuildPairIds(int count)
    {
        var ids = new List<int>(count);
        int pairCount = count / 2;
        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        if (count % 2 != 0)
            ids.Add(-1);

        Shuffle(ids);
        return ids;
    }

    private static void Shuffle(List<int> list)
    {
        var rng = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swap = rng.Next(i + 1);
            (list[i], list[swap]) = (list[swap], list[i]);
        }
    }

    private void HandleCardClicked(int index)
    {
        CardClicked?.Invoke(index);
    }
}
