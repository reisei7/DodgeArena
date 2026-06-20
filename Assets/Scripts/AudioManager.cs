using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Efekty dźwiękowe")]
    public AudioClip shootSound;
    public AudioClip explosionSound;
    public AudioClip[] hitSounds; // 2 warianty trafienia, losowane
    public AudioClip uiClickSound;

    [Header("Ustawienia")]
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayShoot()
    {
        PlaySfx(shootSound);
    }

    public void PlayExplosion()
    {
        PlaySfx(explosionSound);
    }

    public void PlayHit()
    {
        if (hitSounds == null || hitSounds.Length == 0) return;
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        PlaySfx(clip);
    }

    public void PlayUiClick()
    {
        PlaySfx(uiClickSound);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, sfxVolume);
    }
}