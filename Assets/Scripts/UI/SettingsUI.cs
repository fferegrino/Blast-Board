using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Connects a mute Toggle and volume Slider to <see cref="AudioSettings"/>.
/// Assign Mute Toggle and Volume Slider in the inspector, or add children named "soundFxMuteToggle" and "soundFxVolumeSlider".
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Optional – assign in inspector or use child names")]
    public Toggle soundFxMuteToggle;
    public Slider soundFxVolumeSlider;
    [Tooltip("Optional label to show volume as text (e.g. \"50%\").")]
    public TextMeshProUGUI soundFxVolumeLabel;

    public Button closeButton;

    void Start()
    {
        ResolveReferences();
        LoadFromSettings();
        Subscribe();
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
    }

    void LoadFromSettings()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.isOn = AudioSettings.IsMuted;
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.value = AudioSettings.Volume;
        UpdatesoundFxVolumeLabel();
    }

    void Subscribe()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.onValueChanged.AddListener(OnMuteChanged);
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void Unsubscribe()
    {
        if (soundFxMuteToggle != null)
            soundFxMuteToggle.onValueChanged.RemoveListener(OnMuteChanged);
        if (soundFxVolumeSlider != null)
            soundFxVolumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    void OnMuteChanged(bool isOn)
    {
        AudioSettings.IsMuted = isOn;
    }

    void OnVolumeChanged(float value)
    {
        AudioSettings.Volume = value;
        UpdatesoundFxVolumeLabel();
    }

    void UpdatesoundFxVolumeLabel()
    {
        if (soundFxVolumeLabel != null)
            soundFxVolumeLabel.text = Mathf.RoundToInt(AudioSettings.Volume * 100f) + "%";
    }
}
