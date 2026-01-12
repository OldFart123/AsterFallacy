using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;
    private AudioSource MusicSource;

    private void Awake()
    {

        source = GetComponent<AudioSource>();
        MusicSource = transform.GetChild(0).GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        ChangeMusicVolume(0);
        ChangeSoundVolume(0);
    }
    public void PlaySound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }
    public void ChangeSoundVolume(float _change)
    {
        float BaseVolume = 1;

        float currentVolume = PlayerPrefs.GetFloat("SoundVolume", 1);
        currentVolume += _change;

        if (currentVolume > 1)
        {
            currentVolume = 0;
        }
        else if (currentVolume < 0)
        {
            currentVolume = 1;
        }

        float FinalVolume = currentVolume *= BaseVolume;
        source.volume = FinalVolume;

        PlayerPrefs.SetFloat("SoundVolume", currentVolume);
    }
    public void ChangeMusicVolume(float _change)
    {
        float BaseVolume = 0.3f;

        float currentVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        currentVolume += _change;

        if (currentVolume > 1)
        {
            currentVolume = 0;
        }
        else if (currentVolume < 0)
        {
            currentVolume = 1;
        }

        float FinalVolume = currentVolume *= BaseVolume;
        MusicSource.volume = FinalVolume;

        PlayerPrefs.SetFloat("MusicVolume", currentVolume);
    }
}
