using UnityEngine;

/// <summary>
/// Persistent mute and volume settings for game audio.
/// Uses PlayerPrefs so choices survive between sessions.
/// </summary>
public static class AudioSettings
{
    const string KeyMuted = "BoardBlast_AudioMuted";
    const string KeyVolume = "BoardBlast_AudioVolume";

    static bool? _muted;
    static float? _volume;

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

    /// <summary>Effective volume multiplier: 0 when muted, otherwise Volume.</summary>
    public static float EffectiveVolume => IsMuted ? 0f : Volume;
}
