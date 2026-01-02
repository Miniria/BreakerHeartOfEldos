using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Default Audio Clips (Optional)")]
    public AudioClip defaultMusic;
    public AudioClip defaultSFX;

    private void Awake()
    {
        // Singleton สำหรับ Scene นี้เท่านั้น
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // ตรวจสอบว่ามี AudioSource หรือไม่ ถ้าไม่มีให้เพิ่ม
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        // เล่นเพลงเริ่มต้นถ้ามี
        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    /// <summary>
    /// เล่นเพลง (Music)
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            // ถ้าเป็นเพลงเดิมและกำลังเล่นอยู่ ไม่ต้องทำอะไร
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    /// <summary>
    /// เล่นเสียงประกอบ (SFX)
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// หยุดเพลง
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// หยุดเสียง SFX ทั้งหมดที่กำลังเล่นอยู่ (ถ้าใช้ PlayOneShot จะหยุดไม่ได้ทันที)
    /// </summary>
    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }
}
