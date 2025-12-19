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
            installer.cardPrefab == null ||
            installer.levelSelectPrefab == null ||
            installer.hudPrefab == null ||
            installer.homeButtonPrefab == null ||
            installer.nextButtonPrefab == null ||
            installer.levelConfig == null)
        {
            Debug.LogError("AppInstaller: required references are not fully assigned.");
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

        // UI
        var levelSelect = Object.Instantiate(installer.levelSelectPrefab, uiRoot.transform);
        var hud = Object.Instantiate(installer.hudPrefab, uiRoot.transform);
        var home = Object.Instantiate(installer.homeButtonPrefab, uiRoot.transform)
            .GetComponent<HomeButtonView>();
        var next = Object.Instantiate(installer.nextButtonPrefab, uiRoot.transform)
            .GetComponent<NextButtonView>();

        var uiController = uiRoot.GetComponent<GameUIController>();
        if (uiController == null)
            uiController = uiRoot.AddComponent<GameUIController>();

        uiController.Initialize(levelSelect, hud, home, next);

        var statsController = uiRoot.GetComponent<GameStatsController>();
        if (statsController == null)
            statsController = uiRoot.AddComponent<GameStatsController>();

        var gameController = Object.FindFirstObjectByType<GameController>();
        var bus = gameController != null ? gameController.Bus : null;
        statsController.Initialize(hud, bus);

        var session = uiRoot.GetComponent<GameSessionController>();
        if (session == null)
            session = uiRoot.AddComponent<GameSessionController>();

        session.Initialize(board, installer.boardSettings, installer.levelConfig, uiController, installer.cardPrefab, statsController);
        session.ShowMenu();
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
