using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip gameOverClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.45f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.85f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureSources();
    }

    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PauseMusic()
    {
        musicSource?.Pause();
    }

    public void ResumeMusic()
    {
        musicSource?.UnPause();
    }

    public void StopMusic()
    {
        musicSource?.Stop();
    }

    public void PlayCoin()
    {
        PlaySfx(coinClip);
    }

    public void PlayJump()
    {
        PlaySfx(jumpClip);
    }

    public void PlayGameOver()
    {
        PlaySfx(gameOverClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            GameObject musicObject = new GameObject("MusicSource");
            musicObject.transform.SetParent(transform);
            musicSource = musicObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            GameObject sfxObject = new GameObject("SfxSource");
            sfxObject.transform.SetParent(transform);
            sfxSource = sfxObject.AddComponent<AudioSource>();
        }
    }
}
