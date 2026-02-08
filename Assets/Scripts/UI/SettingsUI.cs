using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Connects mute toggles and volume sliders to <see cref="AudioSettings"/> (SFX and music).
/// Assign in the inspector or use child names: soundFxMuteToggle, soundFxVolumeSlider, musicMuteToggle, musicVolumeSlider.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Sound FX – optional, or use child names")]
    public Toggle soundFxMuteToggle;
    public Slider soundFxVolumeSlider;
    [Tooltip("Optional label to show SFX volume (e.g. \"50%\").")]
    public TextMeshProUGUI soundFxVolumeLabel;

    [Header("Music – optional, or use child names")]
    public Toggle musicMuteToggle;
    public Slider musicVolumeSlider;
    [Tooltip("Optional label to show music volume (e.g. \"70%\").")]
    public TextMeshProUGUI musicVolumeLabel;

    public Button closeButton;

    void Start()
    {
        ResolveReferences();
        LoadFromSettings();
        Subscribe();
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    void ResolveReferences()
    {
        if (soundFxMuteToggle == null)
            soundFxMuteToggle = GetComponentsInChildren<Toggle>(true).FirstOrDefault(t => t.name == "soundFxMuteToggle");
        if (soundFxVolumeSlider == null)
            soundFxVolumeSlider = GetComponentsInChildren<Slider>(true).FirstOrDefault(s => s.name == "soundFxVolumeSlider");
        if (soundFxVolumeLabel == null)
            soundFxVolumeLabel = GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "soundFxVolumeLabel");
        if (musicMuteToggle == null)
            musicMuteToggle = GetComponentsInChildren<Toggle>(true).FirstOrDefault(t => t.name == "musicMuteToggle");
        if (musicVolumeSlider == null)
            musicVolumeSlider = GetComponentsInChildren<Slider>(true).FirstOrDefault(s => s.name == "musicVolumeSlider");
        if (musicVolumeLabel == null)
            musicVolumeLabel = GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "musicVolumeLabel");
    }

    void LoadFromSettings()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.isOn = AudioSettings.IsMuted;
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.value = AudioSettings.Volume;
        if (musicMuteToggle != null)
            musicMuteToggle.isOn = AudioSettings.MusicMuted;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = AudioSettings.MusicVolume;
        UpdateSoundFxVolumeLabel();
        UpdateMusicVolumeLabel();
    }

    void Subscribe()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.onValueChanged.AddListener(OnSoundFxMuteChanged);
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.onValueChanged.AddListener(OnSoundFxVolumeChanged);
        if (musicMuteToggle != null)
            musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    void Unsubscribe()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.onValueChanged.RemoveListener(OnSoundFxMuteChanged);
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.onValueChanged.RemoveListener(OnSoundFxVolumeChanged);
        if (musicMuteToggle != null)
            musicMuteToggle.onValueChanged.RemoveListener(OnMusicMuteChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
    }

    void OnSoundFxMuteChanged(bool isOn)
    {
        AudioSettings.IsMuted = isOn;
    }

    void OnSoundFxVolumeChanged(float value)
    {
        AudioSettings.Volume = value;
        UpdateSoundFxVolumeLabel();
    }

    void OnMusicMuteChanged(bool isOn)
    {
        AudioSettings.MusicMuted = isOn;
        if (BackgroundMusicManager.Instance != null)
            BackgroundMusicManager.Instance.RefreshFromSettings();
    }

    void OnMusicVolumeChanged(float value)
    {
        AudioSettings.MusicVolume = value;
        UpdateMusicVolumeLabel();
        if (BackgroundMusicManager.Instance != null)
            BackgroundMusicManager.Instance.RefreshFromSettings();
    }

    void UpdateSoundFxVolumeLabel()
    {
        if (soundFxVolumeLabel != null)
            soundFxVolumeLabel.text = Mathf.RoundToInt(AudioSettings.Volume * 100f) + "%";
    }

    void UpdateMusicVolumeLabel()
    {
        if (musicVolumeLabel != null)
            musicVolumeLabel.text = Mathf.RoundToInt(AudioSettings.MusicVolume * 100f) + "%";
    }
}
