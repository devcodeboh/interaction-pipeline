using UnityEngine;

[DisallowMultipleComponent]
public sealed class InstallerProvider : MonoBehaviour
{
    public static InstallerProvider Instance { get; private set; }

    [SerializeField] private AppInstaller installer;
    public AppInstaller Installer => installer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (installer == null)
        {
            Debug.LogError("InstallerProvider: AppInstaller is not assigned.");
            enabled = false;
        }
    }
}
