using UnityEngine;

/// <summary>
/// Plays looping background music. Respects <see cref="AudioSettings"/> music mute and volume.
/// Assign the initial clip and call <see cref="Play"/> from your menu or game start; use <see cref="RefreshFromSettings"/> when the user changes music settings.
/// </summary>
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Tooltip("Optional: clip to play on Start. You can also call Play() with a clip.")]
    public AudioClip defaultClip;

    AudioSource _source;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = gameObject.GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;
    }

    void Start()
    {
        ApplySettings();
        if (defaultClip != null)
            Play(defaultClip);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Plays the given clip as background music (looping). Does nothing if music is muted.</summary>
    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        _source.clip = clip;
        ApplySettings();
        if (!AudioSettings.MusicMuted)
            _source.Play();
    }

    public void PlayDefault()
    {
        if (defaultClip != null)
            Play(defaultClip);
    }

    /// <summary>Stops background music.</summary>
    public void Stop()
    {
        _source.Stop();
    }

    /// <summary>Call this when the user changes music mute or volume in settings so the current track reflects the new values.</summary>
    public void RefreshFromSettings()
    {
        ApplySettings();
        if (AudioSettings.MusicMuted && _source.isPlaying)
            _source.Pause();
        else if (!AudioSettings.MusicMuted && _source.clip != null && !_source.isPlaying)
            _source.UnPause();
    }

    void ApplySettings()
    {
        _source.volume = AudioSettings.MusicEffectiveVolume;
        if (AudioSettings.MusicMuted && _source.isPlaying)
            _source.Pause();
        else if (!AudioSettings.MusicMuted && _source.clip != null)
        {
            _source.UnPause();
            if (!_source.isPlaying)
                _source.Play();
        }
    }
}
