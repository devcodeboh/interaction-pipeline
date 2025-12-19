using UnityEngine;

public sealed class AudioController : MonoBehaviour
{
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioConfig config;
    private EventBus bus;

    public void Initialize(AudioConfig config, EventBus bus)
    {
        this.config = config;
        this.bus = bus;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        if (this.bus != null)
        {
            this.bus.OnCardFlipStarted += HandleFlip;
            this.bus.OnCardMatchResolved += HandleMatch;
            this.bus.OnCardMismatchResolved += HandleMismatch;
            this.bus.OnGameCompleted += HandleGameCompleted;
        }

        PlayMusic();
    }

    private void OnDestroy()
    {
        if (bus == null)
            return;

        bus.OnCardFlipStarted -= HandleFlip;
        bus.OnCardMatchResolved -= HandleMatch;
        bus.OnCardMismatchResolved -= HandleMismatch;
        bus.OnGameCompleted -= HandleGameCompleted;
    }

    private void HandleFlip(GameEvents.CardFlipStarted data)
    {
        if (data.FaceUp)
            PlaySfx(config != null ? config.flip : null);
    }

    private void HandleMatch(GameEvents.CardMatchResolved _) => PlaySfx(config != null ? config.match : null);
    private void HandleMismatch(GameEvents.CardMismatchResolved _) => PlaySfx(config != null ? config.mismatch : null);
    private void HandleGameCompleted(GameEvents.GameCompleted _) => PlaySfx(config != null ? config.gameCompleted : null);

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    private void PlayMusic()
    {
        if (musicSource == null || config == null || config.music == null)
            return;

        musicSource.clip = config.music;
        musicSource.Play();
    }
}
