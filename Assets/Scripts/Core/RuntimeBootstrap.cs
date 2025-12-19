using UnityEngine;
using UnityEngine.EventSystems;

public static class RuntimeBootstrap
{
    private const string AppRootName = "[AppRoot]";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        // Prevent duplicate bootstrapping (e.g., scene reload)
        if (GameObject.Find(AppRootName) != null)
            return;

        var provider = InstallerProvider.Instance;
        if (provider == null || provider.Installer == null)
        {
            Debug.LogError(
                "RuntimeBootstrap: InstallerProvider/AppInstaller missing. Ensure InstallerProvider exists in the стартовой сцене."
            );
            return;
        }

        var installer = provider.Installer;

        if (installer.uiRootPrefab == null)
        {
            Debug.LogError("AppInstaller: uiRootPrefab is not assigned.");
            return;
        }

        if (installer.boardRootPrefab == null ||
            installer.boardSettings == null ||
            installer.cardPrefab == null)
        {
            Debug.LogError("AppInstaller: board references are not fully assigned.");
            return;
        }

        // Root for ALL runtime objects
        var root = new GameObject(AppRootName);
        Object.DontDestroyOnLoad(root);

        // Parent the provider under the runtime root
        provider.transform.SetParent(root.transform, false);

        // Core
        var gameControllerGo = new GameObject("GameController");
        gameControllerGo.transform.SetParent(root.transform, false);
        gameControllerGo.AddComponent<GameController>();

        // UI Root (Canvas + Scaler + Raycaster)
        var uiRoot = Object.Instantiate(installer.uiRootPrefab);
        uiRoot.transform.SetParent(root.transform, false);

        // Ensure EventSystem exists
        EnsureEventSystem(root.transform);

        // Board
        var board = Object.Instantiate(installer.boardRootPrefab);
        board.transform.SetParent(uiRoot.transform, false);
        board.Initialize(installer.boardSettings, installer.cardPrefab);
    }

    private static void EnsureEventSystem(Transform parent)
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.transform.SetParent(parent, false);

        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }
}
