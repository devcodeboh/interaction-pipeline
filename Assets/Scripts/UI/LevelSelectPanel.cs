using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button playButton;

    public event Action<LevelDifficulty> OnDifficultySelected;
    public event Action OnPlayRequested;

    private void OnEnable()
    {
        if (easyButton == null || mediumButton == null || hardButton == null || playButton == null)
            Debug.LogWarning("LevelSelectPanel: missing button references.");

        if (easyButton != null) easyButton.onClick.AddListener(() => Select(LevelDifficulty.Easy));
        if (mediumButton != null) mediumButton.onClick.AddListener(() => Select(LevelDifficulty.Medium));
        if (hardButton != null) hardButton.onClick.AddListener(() => Select(LevelDifficulty.Hard));
        if (playButton != null) playButton.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        if (easyButton != null) easyButton.onClick.RemoveAllListeners();
        if (mediumButton != null) mediumButton.onClick.RemoveAllListeners();
        if (hardButton != null) hardButton.onClick.RemoveAllListeners();
        if (playButton != null) playButton.onClick.RemoveAllListeners();
    }

    private void Select(LevelDifficulty difficulty) => OnDifficultySelected?.Invoke(difficulty);

    private void Play() => OnPlayRequested?.Invoke();
}
