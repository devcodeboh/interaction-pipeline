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
    private Vector2Int gridSize;

    public IReadOnlyList<CardView> Views => spawnedCards;
    public IReadOnlyList<CardModel> Models => models;
    public Vector2Int GridSize => gridSize;

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
        this.gridSize = EnsurePlayableGrid(gridSize);
        ConfigureGrid(this.gridSize);
        inputController = new CardInputController(models, spawnedCards, bus);
        SpawnCards(this.gridSize, null);
    }

    public void BuildBoardFromSave(Vector2Int gridSize, int[] pairIds, int[] cardStates)
    {
        ClearBoard();
        this.gridSize = gridSize;
        ConfigureGrid(this.gridSize);
        inputController = new CardInputController(models, spawnedCards, bus);
        SpawnCards(this.gridSize, pairIds);
        ApplyCardStates(cardStates);
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

    private void SpawnCards(Vector2Int gridSize, int[] pairIds)
    {
        int count = Mathf.Max(1, gridSize.x) * Mathf.Max(1, gridSize.y);
        var pairs = pairIds != null && pairIds.Length == count ? pairIds : BuildPairIds(count);
        for (int i = 0; i < count; i++)
        {
            var card = UnityEngine.Object.Instantiate(cardPrefab, grid.transform);
            card.Bind(i);
            card.Clicked += inputController.HandleCardClicked;
            card.SetInstant(false);
            spawnedCards.Add(card);
            int pairId = pairs[i];
            models.Add(new CardModel(i, pairId));
            card.SetBackSprite(settings.backSprite);
            card.SetFaceSprite(GetFaceSprite(pairId));
        }
    }

    private void ClearBoard()
    {
        foreach (var card in spawnedCards)
        {
            if (card == null)
                continue;

            if (inputController != null)
                card.Clicked -= inputController.HandleCardClicked;
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
        var sprites = settings.faceSprites;
        if (sprites == null || sprites.Length == 0)
            return null;

        if (pairId < 0 || pairId >= sprites.Length)
            return null;

        return sprites[pairId];
    }

    private Vector2Int EnsurePlayableGrid(Vector2Int gridSize)
    {
        int columns = Mathf.Max(1, gridSize.x);
        int rows = Mathf.Max(1, gridSize.y);
        int spritePairs = settings.faceSprites == null ? 0 : settings.faceSprites.Length;
        int count = columns * rows;

        if (count % 2 != 0)
        {
            if (columns <= rows)
                columns += 1;
            else
                rows += 1;
        }

        int maxPairs = Mathf.Max(1, spritePairs);
        while (columns * rows / 2 > maxPairs && columns > 1 && rows > 1)
        {
            if (columns >= rows)
                columns -= 1;
            else
                rows -= 1;
        }

        var adjusted = new Vector2Int(columns, rows);
        if (adjusted != gridSize)
        {
            Debug.LogWarning(
                $"Board size {gridSize.x}x{gridSize.y} adjusted to {columns}x{rows} (pairs: {columns * rows / 2}, sprites: {spritePairs})."
            );
        }

        return adjusted;
    }

    private static int[] BuildPairIds(int count)
    {
        var ids = new int[count];
        int pairCount = count / 2;
        int index = 0;
        for (int i = 0; i < pairCount; i++)
        {
            ids[index++] = i;
            ids[index++] = i;
        }

        Shuffle(ids);
        return ids;
    }

    private static void Shuffle(int[] list)
    {
        for (int i = list.Length - 1; i > 0; i--)
        {
            int swap = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[swap]) = (list[swap], list[i]);
        }
    }

    public int[] GetPairIds()
    {
        int[] pairIds = new int[models.Count];
        for (int i = 0; i < models.Count; i++)
            pairIds[i] = models[i].PairId;

        return pairIds;
    }

    public int[] GetCardStates()
    {
        int[] states = new int[models.Count];
        for (int i = 0; i < models.Count; i++)
            states[i] = (int)models[i].State;

        return states;
    }

    public int GetMatchedCount()
    {
        int count = 0;
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].State == CardState.Matched)
                count++;
        }

        return count;
    }

    private void ApplyCardStates(int[] cardStates)
    {
        if (cardStates == null || cardStates.Length != models.Count)
            return;

        for (int i = 0; i < models.Count; i++)
        {
            var model = models[i];
            var view = spawnedCards[i];
            if (model == null || view == null)
                continue;

            var state = (CardState)cardStates[i];
            model.SetState(state);

            switch (state)
            {
                case CardState.FaceUp:
                    view.SetInstant(true);
                    break;
                case CardState.Matched:
                    view.SetInstant(true);
                    view.SetMatchedHidden(true);
                    break;
                default:
                    view.SetInstant(false);
                    break;
            }
        }
    }

}
