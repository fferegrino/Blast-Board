using UnityEngine;

public class SoundFXManager: MonoBehaviour
{
    public static SoundFXManager Instance;

    public AudioSource soundFxObject;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySound(AudioClip audioClip, Transform parent, float volume = 1.0f)
    {
        var audioSource = Instantiate(soundFxObject, parent);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        Destroy(audioSource.gameObject, audioClip.length);
    }

}
