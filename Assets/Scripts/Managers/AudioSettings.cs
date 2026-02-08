using UnityEngine;

/// <summary>
/// Persistent mute and volume settings for game audio (SFX and music).
/// Uses PlayerPrefs so choices survive between sessions.
/// </summary>
public static class AudioSettings
{
    const string KeyMuted = "BoardBlast_AudioMuted";
    const string KeyVolume = "BoardBlast_AudioVolume";
    const string KeyMusicMuted = "BoardBlast_MusicMuted";
    const string KeyMusicVolume = "BoardBlast_MusicVolume";

    static bool? _muted;
    static float? _volume;
    static bool? _musicMuted;
    static float? _musicVolume;

    // ---- Sound FX ----
    /// <summary>True when sound effects are muted.</summary>
    public static bool IsMuted
    {
        get
        {
            if (!_muted.HasValue)
                _muted = PlayerPrefs.GetInt(KeyMuted, 0) != 0;
            return _muted.Value;
        }
        set
        {
            _muted = value;
            PlayerPrefs.SetInt(KeyMuted, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Volume for sound effects, 0 to 1.</summary>
    public static float Volume
    {
        get
        {
            if (!_volume.HasValue)
                _volume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyVolume, 1f));
            return _volume.Value;
        }
        set
        {
            _volume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyVolume, _volume.Value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Effective volume multiplier for SFX: 0 when muted, otherwise Volume.</summary>
    public static float EffectiveVolume => IsMuted ? 0f : Volume;

    // ---- Music ----
    /// <summary>True when background music is muted.</summary>
    public static bool MusicMuted
    {
        get
        {
            if (!_musicMuted.HasValue)
                _musicMuted = PlayerPrefs.GetInt(KeyMusicMuted, 0) != 0;
            return _musicMuted.Value;
        }
        set
        {
            _musicMuted = value;
            PlayerPrefs.SetInt(KeyMusicMuted, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Volume for background music, 0 to 1.</summary>
    public static float MusicVolume
    {
        get
        {
            if (!_musicVolume.HasValue)
                _musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyMusicVolume, 0.7f));
            return _musicVolume.Value;
        }
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyMusicVolume, _musicVolume.Value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Effective volume multiplier for music: 0 when muted, otherwise MusicVolume.</summary>
    public static float MusicEffectiveVolume => MusicMuted ? 0f : MusicVolume;
}
