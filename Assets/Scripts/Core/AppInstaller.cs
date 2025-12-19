using UnityEngine;

[CreateAssetMenu(menuName = "Config/App Installer", fileName = "AppInstaller")]
public sealed class AppInstaller : ScriptableObject
{
    [Header("UI")]
    public GameObject uiRootPrefab;
    public LevelSelectPanel levelSelectPrefab;
    public HudView hudPrefab;
    public GameObject homeButtonPrefab;
    public GameObject nextButtonPrefab;
    public WinPopupView winPopupPrefab;

    [Header("Levels")]
    public LevelConfig levelConfig;

    [Header("Board")]
    public BoardControllerBehaviour boardRootPrefab;
    public BoardSettings boardSettings;
    public CardView cardPrefab;
}
