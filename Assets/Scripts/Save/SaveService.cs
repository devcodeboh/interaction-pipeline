using UnityEngine;

public sealed class SaveService
{
    private const string SaveKey = "memory_save_v1";

    public bool HasSave() => PlayerPrefs.HasKey(SaveKey);

    public void Save(GameSaveData data)
    {
        if (data == null)
            return;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public GameSaveData Load()
    {
        if (!HasSave())
            return null;

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public void Clear()
    {
        if (HasSave())
            PlayerPrefs.DeleteKey(SaveKey);
    }
}
