using UnityEngine;

[CreateAssetMenu(menuName = "Config/App Installer", fileName = "AppInstaller")]
public sealed class AppInstaller : ScriptableObject
{
    [Header("UI")]
    public GameObject uiRootPrefab;

    [Header("Board")]
    public BoardControllerBehaviour boardRootPrefab;
    public BoardSettings boardSettings;
    public CardView cardPrefab;
}
