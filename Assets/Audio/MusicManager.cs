using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Default Music")]
    public AudioClip defaultMusic;

    private AudioSource source;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f; 
        source.volume = 0.1f;
    }

    void Start()
    {
        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic, 0.1f, 20f);
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 0.1f, float fadeDuration = 20f)
    {
        if (clip == null)
        {
            Debug.LogWarning("MusicManager.PlayMusic called with null clip");
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        fadeRoutine = StartCoroutine(FadeInMusic(clip, volume, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 15f)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutMusic(fadeDuration));
    }

    private IEnumerator FadeInMusic(AudioClip clip, float targetVolume, float duration)
    {
        source.clip = clip;
        source.volume = 0f;
        source.Play();

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
        fadeRoutine = null;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.Stop();
        source.volume = 0.1f;
        fadeRoutine = null;
    }
}