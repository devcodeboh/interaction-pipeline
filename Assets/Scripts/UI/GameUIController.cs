using System;
using UnityEngine;

public sealed class GameUIController : MonoBehaviour
{
    [SerializeField] private LevelSelectPanel levelSelect;
    [SerializeField] private HudView hud;
    [SerializeField] private HomeButtonView homeButton;
    [SerializeField] private NextButtonView nextButton;
    [SerializeField] private WinPopupView winPopup;

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
        else
        {
            Debug.LogWarning("GameUIController: LevelSelectPanel is missing.");
        }

        if (this.homeButton != null)
            this.homeButton.Clicked += HandleHomeRequested;
        else
            Debug.LogWarning("GameUIController: HomeButtonView is missing.");

        if (this.nextButton != null)
            this.nextButton.Clicked += HandleNextRequested;
        else
            Debug.LogWarning("GameUIController: NextButtonView is missing.");
    }

    public void RegisterWinPopup(WinPopupView popup)
    {
        winPopup = popup;
        if (winPopup == null)
            return;

        winPopup.Hide();
        if (winPopup.NextButton != null)
            winPopup.NextButton.Clicked += HandleNextRequested;
    }

    public void ShowMenu()
    {
        if (levelSelect != null) levelSelect.gameObject.SetActive(true);
        if (hud != null) hud.gameObject.SetActive(false);
        if (homeButton != null) homeButton.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (winPopup != null) winPopup.Hide();
    }

    public void ShowHud()
    {
        if (levelSelect != null) levelSelect.gameObject.SetActive(false);
        if (hud != null) hud.gameObject.SetActive(true);
        if (homeButton != null) homeButton.gameObject.SetActive(true);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        if (winPopup != null) winPopup.Hide();
    }

    public void ShowWinPopup()
    {
        if (winPopup != null)
            winPopup.Show();
    }

    private void HandleDifficultySelected(LevelDifficulty difficulty) => DifficultySelected?.Invoke(difficulty);
    private void HandlePlayRequested() => PlayRequested?.Invoke();
    private void HandleHomeRequested() => HomeRequested?.Invoke();
    private void HandleNextRequested() => NextRequested?.Invoke();
}
