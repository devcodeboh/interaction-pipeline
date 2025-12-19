using UnityEngine;

[CreateAssetMenu(menuName = "Config/Audio Config", fileName = "AudioConfig")]
public sealed class AudioConfig : ScriptableObject
{
    public AudioClip flip;
    public AudioClip match;
    public AudioClip mismatch;
    public AudioClip gameCompleted;
    public AudioClip music;
}
