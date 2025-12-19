using System;
using UnityEngine;

public sealed class GameUIController : MonoBehaviour
{
    [SerializeField] private LevelSelectPanel levelSelect;
    [SerializeField] private HudView hud;
    [SerializeField] private HomeButtonView homeButton;
    [SerializeField] private NextButtonView nextButton;

    public event Action<LevelDifficulty> DifficultySelected;
    public event Action PlayRequested;
    public event Action HomeRequested;
    public event Action NextRequested;

    public void Initialize(LevelSelectPanel levelSelect, HudView hud, HomeButtonView homeButton, NextButtonView nextButton)
    {
        this.levelSelect = levelSelect;
        this.hud = hud;
        this.homeButton = homeButton;
        this.nextButton = nextButton;

        if (this.levelSelect != null)
        {
            this.levelSelect.OnDifficultySelected += HandleDifficultySelected;
            this.levelSelect.OnPlayRequested += HandlePlayRequested;
        }

        if (this.homeButton != null)
            this.homeButton.Clicked += HandleHomeRequested;

        if (this.nextButton != null)
            this.nextButton.Clicked += HandleNextRequested;
    }

    public void ShowMenu()
    {
        if (levelSelect != null) levelSelect.gameObject.SetActive(true);
        if (hud != null) hud.gameObject.SetActive(false);
        if (homeButton != null) homeButton.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false);
    }

    public void ShowHud()
    {
        if (levelSelect != null) levelSelect.gameObject.SetActive(false);
        if (hud != null) hud.gameObject.SetActive(true);
        if (homeButton != null) homeButton.gameObject.SetActive(true);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    private void HandleDifficultySelected(LevelDifficulty difficulty) => DifficultySelected?.Invoke(difficulty);
    private void HandlePlayRequested() => PlayRequested?.Invoke();
    private void HandleHomeRequested() => HomeRequested?.Invoke();
    private void HandleNextRequested() => NextRequested?.Invoke();
}
