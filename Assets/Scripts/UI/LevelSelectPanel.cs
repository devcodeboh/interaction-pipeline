using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private Toggle easyToggle;
    [SerializeField] private Toggle mediumToggle;
    [SerializeField] private Toggle hardToggle;
    [SerializeField] private Button playButton;

    public event Action<LevelDifficulty> OnDifficultySelected;
    public event Action OnPlayRequested;

    private void OnEnable()
    {
        if (easyToggle == null || mediumToggle == null || hardToggle == null || playButton == null)
            Debug.LogWarning("LevelSelectPanel: missing toggle/button references.");

        if (easyToggle != null) easyToggle.onValueChanged.AddListener(isOn => HandleToggle(LevelDifficulty.Easy, isOn));
        if (mediumToggle != null) mediumToggle.onValueChanged.AddListener(isOn => HandleToggle(LevelDifficulty.Medium, isOn));
        if (hardToggle != null) hardToggle.onValueChanged.AddListener(isOn => HandleToggle(LevelDifficulty.Hard, isOn));
        if (playButton != null) playButton.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        if (easyToggle != null) easyToggle.onValueChanged.RemoveAllListeners();
        if (mediumToggle != null) mediumToggle.onValueChanged.RemoveAllListeners();
        if (hardToggle != null) hardToggle.onValueChanged.RemoveAllListeners();
        if (playButton != null) playButton.onClick.RemoveAllListeners();
    }

    private void HandleToggle(LevelDifficulty difficulty, bool isOn)
    {
        if (isOn)
            OnDifficultySelected?.Invoke(difficulty);
    }

    private void Play() => OnPlayRequested?.Invoke();
}
