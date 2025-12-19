using UnityEngine;
using UnityEngine.UI;

public sealed class BoardControllerBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform boardContainer;
    [SerializeField] private GridLayoutGroup grid;

    private BoardController controller;

    public void Initialize(BoardSettings settings, CardView cardPrefab)
    {
        controller = new BoardController(boardContainer, grid, settings, cardPrefab);
        controller.BuildBoard(settings.gridSize);
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
