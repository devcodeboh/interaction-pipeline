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
        for (int i = 0; i < count; i++)
        {
            var card = Object.Instantiate(cardPrefab, grid.transform);
            card.Bind(i);
            card.Clicked += HandleCardClicked;
            card.SetInstant(false);
            spawnedCards.Add(card);
            models.Add(new CardModel(i, i / 2));
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
            Object.Destroy(card.gameObject);
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

    private void HandleCardClicked(int index)
    {
        CardClicked?.Invoke(index);
    }
}
